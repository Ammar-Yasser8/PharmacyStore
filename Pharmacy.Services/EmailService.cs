using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Pharmacy.Services.Dtos.OrderDtos;
using System.Text;

namespace Pharmacy.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        // ── Send to Admin when a new order is placed ──
        public async Task SendOrderCreatedToAdminAsync(OrderToReturnDto order)
        {
            var subject = $"🛒 New Order #{order.Id} from {order.BuyerName}";
            var body = BuildOrderCreatedAdminBody(order);
            await SendEmailAsync(_settings.AdminEmail, subject, body);
        }

        // ── Send to User when their order is placed ──
        public async Task SendOrderCreatedToUserAsync(OrderToReturnDto order)
        {
            var subject = $"✅ Order #{order.Id} Placed Successfully — Ellaithy Pharmacy";
            var body = BuildOrderCreatedUserBody(order);
            await SendEmailAsync(order.BuyerEmail, subject, body);
        }

        // ── Send to User when order is confirmed ──
        public async Task SendOrderConfirmedToUserAsync(OrderToReturnDto order)
        {
            var subject = $"✅ Order #{order.Id} Confirmed — Ellaithy Pharmacy";
            var body = BuildOrderConfirmedBody(order);
            await SendEmailAsync(order.BuyerEmail, subject, body);
        }

        // ── Send to User when order is shipped ──
        public async Task SendOrderShippedToUserAsync(OrderToReturnDto order)
        {
            var subject = $"🚚 Order #{order.Id} Shipped — Ellaithy Pharmacy";
            var body = BuildOrderShippedBody(order);
            await SendEmailAsync(order.BuyerEmail, subject, body);
        }

        // ── Send to User when order is delivered ──
        public async Task SendOrderDeliveredToUserAsync(OrderToReturnDto order)
        {
            var subject = $"🎁 Order #{order.Id} Delivered — Ellaithy Pharmacy";
            var body = BuildOrderDeliveredBody(order);
            await SendEmailAsync(order.BuyerEmail, subject, body);
        }

        // ── Send to User when order is cancelled ──
        public async Task SendOrderCancelledToUserAsync(OrderToReturnDto order)
        {
            var subject = $"❌ Order #{order.Id} Cancelled — Ellaithy Pharmacy";
            var body = BuildOrderCancelledBody(order);
            await SendEmailAsync(order.BuyerEmail, subject, body);
        }

        // ═══════════════════════════════════════════
        //  Private Helpers
        // ═══════════════════════════════════════════

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email} — Subject: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} — Subject: {Subject}", toEmail, subject);
                // Don't throw — email failure should NOT break the order flow
            }
        }

        // ── HTML Templates ──

        private string BuildOrderCreatedAdminBody(OrderToReturnDto order)
        {
            var items = BuildItemsTable(order);
            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin: 0; padding: 20px;'>
  <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 12px rgba(0,0,0,0.08);'>
    <div style='background: linear-gradient(135deg, #1e3a5f, #27ae60); padding: 30px; text-align: center;'>
      <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>🛒 New Order Received</h1>
    </div>
    <div style='padding: 30px;'>
      <p style='font-size: 16px; color: #333;'>A new order has been placed on <strong>Ellaithy Pharmacy</strong>.</p>
      <table style='width: 100%; border-collapse: collapse; margin: 20px 0;'>
        <tr><td style='padding: 8px 0; color: #666;'>Order ID</td><td style='padding: 8px 0; font-weight: bold; color: #1e3a5f;'>#{order.Id}</td></tr>
        <tr><td style='padding: 8px 0; color: #666;'>Customer</td><td style='padding: 8px 0; font-weight: bold;'>{order.BuyerName}</td></tr>
        <tr><td style='padding: 8px 0; color: #666;'>Email</td><td style='padding: 8px 0;'>{order.BuyerEmail}</td></tr>
        <tr><td style='padding: 8px 0; color: #666;'>Phone</td><td style='padding: 8px 0;'>{order.PhoneNumber}</td></tr>
        <tr><td style='padding: 8px 0; color: #666;'>Address</td><td style='padding: 8px 0;'>{order.ShipToAddress.Street}, {order.ShipToAddress.Area}, {order.ShipToAddress.City}</td></tr>
        <tr><td style='padding: 8px 0; color: #666;'>Date</td><td style='padding: 8px 0;'>{order.OrderDate:yyyy-MM-dd HH:mm}</td></tr>
      </table>
      <h3 style='color: #1e3a5f; border-bottom: 2px solid #27ae60; padding-bottom: 8px;'>Order Items</h3>
      {items}
      <table style='width: 100%; margin-top: 20px; border-collapse: collapse;'>
        <tr><td style='padding: 6px 0; color: #666;'>Subtotal</td><td style='padding: 6px 0; text-align: right; font-weight: bold;'>{order.Subtotal:F2} EGP</td></tr>
        <tr><td style='padding: 6px 0; color: #666;'>Shipping</td><td style='padding: 6px 0; text-align: right; font-weight: bold;'>{order.ShippingFee:F2} EGP</td></tr>
        <tr style='border-top: 2px solid #27ae60;'><td style='padding: 10px 0; color: #1e3a5f; font-size: 18px; font-weight: bold;'>Total</td><td style='padding: 10px 0; text-align: right; color: #27ae60; font-size: 18px; font-weight: bold;'>{order.Total:F2} EGP</td></tr>
      </table>
    </div>
    <div style='background: #f4f6f8; padding: 15px; text-align: center; color: #999; font-size: 12px;'>
      Ellaithy Pharmacy — Admin Notification
    </div>
  </div>
</body>
</html>";
        }

        private string BuildOrderCreatedUserBody(OrderToReturnDto order)
        {
            var items = BuildItemsTable(order);
            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin: 0; padding: 20px;'>
  <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 12px rgba(0,0,0,0.08);'>
    <div style='background: linear-gradient(135deg, #27ae60, #2ecc71); padding: 30px; text-align: center;'>
      <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>✅ Order Placed Successfully!</h1>
    </div>
    <div style='padding: 30px;'>
      <p style='font-size: 16px; color: #333;'>Hello <strong>{order.BuyerName}</strong>,</p>
      <p style='color: #555;'>Thank you for your order! We've received it and will process it shortly.</p>
      <div style='background: #e8f5e9; border-radius: 8px; padding: 15px; margin: 15px 0;'>
        <p style='margin: 0; color: #2e7d32; font-weight: bold;'>Order #{order.Id}</p>
        <p style='margin: 5px 0 0; color: #555; font-size: 14px;'>Placed on {order.OrderDate:yyyy-MM-dd HH:mm}</p>
      </div>
      <h3 style='color: #1e3a5f; border-bottom: 2px solid #27ae60; padding-bottom: 8px;'>Your Items</h3>
      {items}
      <table style='width: 100%; margin-top: 20px; border-collapse: collapse;'>
        <tr><td style='padding: 6px 0; color: #666;'>Subtotal</td><td style='padding: 6px 0; text-align: right;'>{order.Subtotal:F2} EGP</td></tr>
        <tr><td style='padding: 6px 0; color: #666;'>Shipping</td><td style='padding: 6px 0; text-align: right;'>{order.ShippingFee:F2} EGP</td></tr>
        <tr style='border-top: 2px solid #27ae60;'><td style='padding: 10px 0; font-size: 18px; font-weight: bold; color: #1e3a5f;'>Total</td><td style='padding: 10px 0; text-align: right; font-size: 18px; font-weight: bold; color: #27ae60;'>{order.Total:F2} EGP</td></tr>
      </table>
      <h3 style='color: #1e3a5f; margin-top: 25px;'>Delivery Address</h3>
      <p style='color: #555;'>{order.ShipToAddress.Street}, {order.ShipToAddress.Area}, {order.ShipToAddress.City}, {order.ShipToAddress.Country}</p>
      <p style='color: #888; font-size: 13px; margin-top: 25px;'>If you have any questions, feel free to contact us.</p>
    </div>
    <div style='background: #f4f6f8; padding: 15px; text-align: center; color: #999; font-size: 12px;'>
      © Ellaithy Pharmacy — Thank you for shopping with us! 💚
    </div>
  </div>
</body>
</html>";
        }

        private string BuildOrderConfirmedBody(OrderToReturnDto order)
        {
            var items = BuildItemsTable(order);
            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin: 0; padding: 20px;'>
  <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 12px rgba(0,0,0,0.08);'>
    <div style='background: linear-gradient(135deg, #1e88e5, #42a5f5); padding: 30px; text-align: center;'>
      <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>✅ Order Confirmed!</h1>
    </div>
    <div style='padding: 30px;'>
      <p style='font-size: 16px; color: #333;'>Hello <strong>{order.BuyerName}</strong>,</p>
      <p style='color: #555;'>Great news! Your order <strong>#{order.Id}</strong> has been <strong>confirmed</strong> and is being prepared for delivery.</p>
      <div style='background: #e3f2fd; border-radius: 8px; padding: 15px; margin: 15px 0;'>
        <p style='margin: 0; color: #1565c0; font-weight: bold; font-size: 16px;'>📦 Order #{order.Id} — Confirmed</p>
        <p style='margin: 5px 0 0; color: #555; font-size: 14px;'>We are preparing your items and will ship them soon.</p>
      </div>
      <h3 style='color: #1e3a5f; border-bottom: 2px solid #1e88e5; padding-bottom: 8px;'>Order Summary</h3>
      {items}
      <table style='width: 100%; margin-top: 20px; border-collapse: collapse;'>
        <tr><td style='padding: 6px 0; color: #666;'>Subtotal</td><td style='padding: 6px 0; text-align: right;'>{order.Subtotal:F2} EGP</td></tr>
        <tr><td style='padding: 6px 0; color: #666;'>Shipping</td><td style='padding: 6px 0; text-align: right;'>{order.ShippingFee:F2} EGP</td></tr>
        <tr style='border-top: 2px solid #1e88e5;'><td style='padding: 10px 0; font-size: 18px; font-weight: bold; color: #1e3a5f;'>Total</td><td style='padding: 10px 0; text-align: right; font-size: 18px; font-weight: bold; color: #1e88e5;'>{order.Total:F2} EGP</td></tr>
      </table>
      <h3 style='color: #1e3a5f; margin-top: 25px;'>Delivery Address</h3>
      <p style='color: #555;'>{order.ShipToAddress.Street}, {order.ShipToAddress.Area}, {order.ShipToAddress.City}, {order.ShipToAddress.Country}</p>
      <p style='color: #888; font-size: 13px; margin-top: 25px;'>You'll receive another email when your order is shipped.</p>
    </div>
    <div style='background: #f4f6f8; padding: 15px; text-align: center; color: #999; font-size: 12px;'>
      © Ellaithy Pharmacy — We're preparing your order! 💊
    </div>
  </div>
</body>
</html>";
        }

        private string BuildOrderShippedBody(OrderToReturnDto order)
        {
            var items = BuildItemsTable(order);
            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin: 0; padding: 20px;'>
  <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 12px rgba(0,0,0,0.08);'>
    <div style='background: linear-gradient(135deg, #f57c00, #ff9800); padding: 30px; text-align: center;'>
      <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>🚚 Order Shipped!</h1>
    </div>
    <div style='padding: 30px;'>
      <p style='font-size: 16px; color: #333;'>Hello <strong>{order.BuyerName}</strong>,</p>
      <p style='color: #555;'>Your order <strong>#{order.Id}</strong> has been <strong>shipped</strong> and is on its way to you!</p>
      <div style='background: #fff3e0; border-radius: 8px; padding: 15px; margin: 15px 0;'>
        <p style='margin: 0; color: #e65100; font-weight: bold; font-size: 16px;'>🚚 Order #{order.Id} — Shipped</p>
        <p style='margin: 5px 0 0; color: #555; font-size: 14px;'>Your order is on its way! Please be available at your delivery address.</p>
      </div>
      <h3 style='color: #1e3a5f; border-bottom: 2px solid #f57c00; padding-bottom: 8px;'>Order Summary</h3>
      {items}
      <table style='width: 100%; margin-top: 20px; border-collapse: collapse;'>
        <tr><td style='padding: 6px 0; color: #666;'>Subtotal</td><td style='padding: 6px 0; text-align: right;'>{order.Subtotal:F2} EGP</td></tr>
        <tr><td style='padding: 6px 0; color: #666;'>Shipping</td><td style='padding: 6px 0; text-align: right;'>{order.ShippingFee:F2} EGP</td></tr>
        <tr style='border-top: 2px solid #f57c00;'><td style='padding: 10px 0; font-size: 18px; font-weight: bold; color: #1e3a5f;'>Total</td><td style='padding: 10px 0; text-align: right; font-size: 18px; font-weight: bold; color: #f57c00;'>{order.Total:F2} EGP</td></tr>
      </table>
      <h3 style='color: #1e3a5f; margin-top: 25px;'>Delivery Address</h3>
      <p style='color: #555;'>{order.ShipToAddress.Street}, {order.ShipToAddress.Area}, {order.ShipToAddress.City}, {order.ShipToAddress.Country}</p>
      <p style='color: #888; font-size: 13px; margin-top: 25px;'>Thank you for choosing Ellaithy Pharmacy!</p>
    </div>
    <div style='background: #f4f6f8; padding: 15px; text-align: center; color: #999; font-size: 12px;'>
      © Ellaithy Pharmacy — Your order is on its way! 🚚
    </div>
  </div>
</body>
</html>";
        }

        private string BuildOrderDeliveredBody(OrderToReturnDto order)
        {
            var items = BuildItemsTable(order);
            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin: 0; padding: 20px;'>
  <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 12px rgba(0,0,0,0.08);'>
    <div style='background: linear-gradient(135deg, #2e7d32, #43a047); padding: 30px; text-align: center;'>
      <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>🎁 Order Delivered!</h1>
    </div>
    <div style='padding: 30px;'>
      <p style='font-size: 16px; color: #333;'>Hello <strong>{order.BuyerName}</strong>,</p>
      <p style='color: #555;'>Your order <strong>#{order.Id}</strong> has been successfully delivered. We hope you are satisfied with your purchase!</p>
      <div style='background: #e8f5e9; border-radius: 8px; padding: 15px; margin: 15px 0;'>
        <p style='margin: 0; color: #2e7d32; font-weight: bold; font-size: 16px;'>✅ Order #{order.Id} — Delivered</p>
        <p style='margin: 5px 0 0; color: #555; font-size: 14px;'>Delivered on {DateTime.Now:yyyy-MM-dd HH:mm}</p>
      </div>
      <h3 style='color: #1e3a5f; border-bottom: 2px solid #2e7d32; padding-bottom: 8px;'>Order Details</h3>
      {items}
      <table style='width: 100%; margin-top: 20px; border-collapse: collapse;'>
        <tr style='border-top: 2px solid #2e7d32;'><td style='padding: 10px 0; font-size: 18px; font-weight: bold; color: #1e3a5f;'>Total Paid</td><td style='padding: 10px 0; text-align: right; font-size: 18px; font-weight: bold; color: #2e7d32;'>{order.Total:F2} EGP</td></tr>
      </table>
      <p style='color: #555; margin-top: 25px;'>If you have any feedback or issues, please don't hesitate to reach out.</p>
      <p style='color: #888; font-size: 13px;'>Thank you for trusting Ellaithy Pharmacy!</p>
    </div>
    <div style='background: #f4f6f8; padding: 15px; text-align: center; color: #999; font-size: 12px;'>
      © Ellaithy Pharmacy — Healthy life, Happy life! 💚
    </div>
  </div>
</body>
</html>";
        }

        private string BuildOrderCancelledBody(OrderToReturnDto order)
        {
            var items = BuildItemsTable(order);
            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f6f8; margin: 0; padding: 20px;'>
  <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 12px rgba(0,0,0,0.08);'>
    <div style='background: linear-gradient(135deg, #c62828, #e53935); padding: 30px; text-align: center;'>
      <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>❌ Order Cancelled</h1>
    </div>
    <div style='padding: 30px;'>
      <p style='font-size: 16px; color: #333;'>Hello <strong>{order.BuyerName}</strong>,</p>
      <p style='color: #555;'>We're writing to inform you that your order <strong>#{order.Id}</strong> has been <strong>cancelled</strong>.</p>
      <div style='background: #ffebee; border-radius: 8px; padding: 15px; margin: 15px 0;'>
        <p style='margin: 0; color: #c62828; font-weight: bold; font-size: 16px;'>⚠️ Order #{order.Id} — Cancelled</p>
        <p style='margin: 5px 0 0; color: #555; font-size: 14px;'>If this was a mistake or you have questions, please contact our support.</p>
      </div>
      <h3 style='color: #1e3a5f; border-bottom: 2px solid #c62828; padding-bottom: 8px;'>Cancelled Items</h3>
      {items}
      <p style='color: #888; font-size: 13px; margin-top: 25px;'>We hope to serve you again in the future.</p>
    </div>
    <div style='background: #f4f6f8; padding: 15px; text-align: center; color: #999; font-size: 12px;'>
      © Ellaithy Pharmacy — Support available 24/7
    </div>
  </div>
</body>
</html>";
        }

        private string BuildItemsTable(OrderToReturnDto order)
        {
            var sb = new StringBuilder();
            sb.Append("<table style='width: 100%; border-collapse: collapse;'>");
            sb.Append("<tr style='background: #f8f9fa;'>");
            sb.Append("<th style='padding: 10px; text-align: left; color: #666; font-size: 13px;'>Product</th>");
            sb.Append("<th style='padding: 10px; text-align: center; color: #666; font-size: 13px;'>Qty</th>");
            sb.Append("<th style='padding: 10px; text-align: right; color: #666; font-size: 13px;'>Price</th>");
            sb.Append("<th style='padding: 10px; text-align: right; color: #666; font-size: 13px;'>Total</th>");
            sb.Append("</tr>");

            foreach (var item in order.OrderItems)
            {
                sb.Append($@"
                <tr style='border-bottom: 1px solid #eee;'>
                    <td style='padding: 10px; color: #333;'>{item.ProductName}</td>
                    <td style='padding: 10px; text-align: center; color: #555;'>{item.Quantity}</td>
                    <td style='padding: 10px; text-align: right; color: #555;'>{item.Price:F2} EGP</td>
                    <td style='padding: 10px; text-align: right; color: #333; font-weight: bold;'>{(item.Price * item.Quantity):F2} EGP</td>
                </tr>");
            }

            sb.Append("</table>");
            return sb.ToString();
        }
    }
}
