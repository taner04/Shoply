using System.Text;
using Shoply.WebApi.Common.Infrastructure.Services.Emails.Templates;

namespace Shoply.WebApi.Features.Orders.Endpoints.CancelOrder;

public sealed class CancelOrderEmailTemplate(string userEmail, Order order) : UserEmailTemplate(userEmail)
{
    public override string Subject { get; } = $"Order canceled - Order #{order.Id}";

    protected override string BuildBody()
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"UTF-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("<style>");
        sb.AppendLine("* { margin: 0; padding: 0; box-sizing: border-box; }");
        sb.AppendLine(
            "body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333; background-color: #f5f5f5; }");
        sb.AppendLine(".email-container { max-width: 600px; margin: 0 auto; background-color: #ffffff; }");
        sb.AppendLine(
            ".header { background: linear-gradient(135deg, #d35400 0%, #c0392b 100%); color: white; padding: 40px 20px; text-align: center; }");
        sb.AppendLine(".header h1 { font-size: 28px; margin-bottom: 10px; }");
        sb.AppendLine(".header p { font-size: 14px; opacity: 0.9; }");
        sb.AppendLine(".content { padding: 40px 20px; }");
        sb.AppendLine(
            ".order-id { background-color: #fff5f5; padding: 15px; border-left: 4px solid #c0392b; margin-bottom: 30px; border-radius: 4px; }");
        sb.AppendLine(".order-id p { font-size: 14px; color: #666; }");
        sb.AppendLine(".order-id .id { font-size: 18px; font-weight: bold; color: #c0392b; }");
        sb.AppendLine(
            "h2 { font-size: 18px; color: #333; margin-bottom: 20px; border-bottom: 2px solid #d35400; padding-bottom: 10px; }");
        sb.AppendLine(".products-table { width: 100%; border-collapse: collapse; margin-bottom: 30px; }");
        sb.AppendLine(
            ".products-table th { background-color: #f5f5f5; padding: 12px; text-align: left; font-weight: 600; border-bottom: 2px solid #ddd; }");
        sb.AppendLine(".products-table td { padding: 12px; border-bottom: 1px solid #eee; }");
        sb.AppendLine(".products-table .price { text-align: right; font-weight: 500; }");
        sb.AppendLine(
            ".summary { background-color: #fff5f5; padding: 20px; border-radius: 4px; margin-bottom: 30px; }");
        sb.AppendLine(
            ".summary-row { display: flex; justify-content: space-between; padding: 8px 0; font-size: 14px; }");
        sb.AppendLine(
            ".summary-row.total { font-size: 18px; font-weight: bold; color: #c0392b; border-top: 2px solid #ddd; padding-top: 12px; }");
        sb.AppendLine(
            ".status-badge { display: inline-block; background-color: #f8d7da; color: #721c24; padding: 8px 16px; border-radius: 20px; font-size: 12px; font-weight: 600; margin-bottom: 20px; }");
        sb.AppendLine(
            ".info-section { background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; border-radius: 4px; margin-bottom: 20px; font-size: 14px; color: #856404; }");
        sb.AppendLine(
            ".refund-info { background-color: #d4edda; padding: 15px; border-left: 4px solid #28a745; border-radius: 4px; margin-bottom: 20px; font-size: 14px; color: #155724; }");
        sb.AppendLine(
            ".footer { background-color: #f5f5f5; padding: 30px 20px; text-align: center; border-top: 1px solid #ddd; }");
        sb.AppendLine(".footer p { font-size: 12px; color: #999; margin-bottom: 10px; }");
        sb.AppendLine(
            ".cta-button { display: inline-block; background-color: #c0392b; color: white; padding: 12px 30px; text-decoration: none; border-radius: 4px; font-weight: 600; margin: 20px 0; }");
        sb.AppendLine(".cta-button:hover { background-color: #a93226; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<div class=\"email-container\">");

        // Header
        sb.AppendLine("<div class=\"header\">");
        sb.AppendLine("<h1>✓ Order Cancelled</h1>");
        sb.AppendLine("<p>Your order has been successfully cancelled</p>");
        sb.AppendLine("</div>");

        // Main Content
        sb.AppendLine("<div class=\"content\">");

        // Status Badge
        sb.AppendLine("<span class=\"status-badge\">STATUS: CANCELLED</span>");

        // Order ID
        sb.AppendLine("<div class=\"order-id\">");
        sb.AppendLine($"<p>Order ID: <span class=\"id\">{order.Id.Value:N}</span></p>");
        sb.AppendLine($"<p>Cancellation Date: {DateTime.UtcNow:MMMM dd, yyyy 'at' hh:mm tt}</p>");
        sb.AppendLine("</div>");

        // Refund Information
        sb.AppendLine("<div class=\"refund-info\">");
        sb.AppendLine("<strong>Refund Status</strong><br>");
        sb.AppendLine($"Your refund of <strong>{order.Payment.Amount:C}</strong> has been initiated. ");
        sb.AppendLine("The funds should appear in your account within 5-7 business days, depending on your bank.");
        sb.AppendLine("</div>");

        // Order Items
        sb.AppendLine("<h2>Cancelled Items</h2>");
        sb.AppendLine("<table class=\"products-table\">");
        sb.AppendLine(
            "<thead><tr><th>Product</th><th>Qty</th><th class=\"price\">Unit Price</th><th class=\"price\">Total</th></tr></thead>");
        sb.AppendLine("<tbody>");
        foreach (var item in order.OrderItems)
        {
            sb.AppendLine(
                $"<tr><td><strong>{item.ProductName}</strong><br><small>{item.ProductDescription}</small></td><td>{item.Quantity}</td><td class=\"price\">{item.UnitPrice:C}</td><td class=\"price\">{item.TotalPrice:C}</td></tr>");
        }

        sb.AppendLine("</tbody>");
        sb.AppendLine("</table>");

        // Order Summary
        sb.AppendLine("<div class=\"summary\">");
        sb.AppendLine("<div class=\"summary-row\">");
        sb.AppendLine($"<span>Subtotal:</span><span>{order.Payment.Amount:C}</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"summary-row\">");
        sb.AppendLine("<span>Shipping:</span><span>-</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"summary-row total\">");
        sb.AppendLine($"<span>Refund Amount:</span><span>{order.Payment.Amount:C}</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("</div>");

        // Need Help
        sb.AppendLine("<div class=\"info-section\">");
        sb.AppendLine("<strong>Need Help?</strong><br>");
        sb.AppendLine("If you didn't request this cancellation or have questions about your refund, ");
        sb.AppendLine("please contact our support team immediately.");
        sb.AppendLine("</div>");

        // CTA
        sb.AppendLine("<center>");
        sb.AppendLine("<a href=\"#\" class=\"cta-button\">Contact Support</a>");
        sb.AppendLine("</center>");

        sb.AppendLine("</div>");

        // Footer
        sb.AppendLine("<div class=\"footer\">");
        sb.AppendLine("<p>Questions about your cancellation? Contact us at support@shoply.com</p>");
        sb.AppendLine("<p>&copy; 2026 Shoply. All rights reserved.</p>");
        sb.AppendLine(
            "<p><a href=\"#\" style=\"color: #c0392b; text-decoration: none;\">Unsubscribe</a> | <a href=\"#\" style=\"color: #c0392b; text-decoration: none;\">Privacy Policy</a></p>");
        sb.AppendLine("</div>");

        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }
}