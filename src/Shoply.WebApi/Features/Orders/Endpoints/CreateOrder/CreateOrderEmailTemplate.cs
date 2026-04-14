using System.Text;
using Shoply.WebApi.Common.Infrastructure.Services.Emails;

namespace Shoply.WebApi.Features.Orders.Endpoints.CreateOrder;

public class CreateOrderEmailTemplate(Order order, string userEmail) : IEmailTemplate
{
    public string To { get; set; } = userEmail;
    public string Subject { get; } = $"Order confirmation - Order #{order.Id}";
    public string Body { get; } = BuildHtmlBody(order);

    private static string BuildHtmlBody(Order order)
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
            ".header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 40px 20px; text-align: center; }");
        sb.AppendLine(".header h1 { font-size: 28px; margin-bottom: 10px; }");
        sb.AppendLine(".header p { font-size: 14px; opacity: 0.9; }");
        sb.AppendLine(".content { padding: 40px 20px; }");
        sb.AppendLine(
            ".order-id { background-color: #f9f9f9; padding: 15px; border-left: 4px solid #667eea; margin-bottom: 30px; border-radius: 4px; }");
        sb.AppendLine(".order-id p { font-size: 14px; color: #666; }");
        sb.AppendLine(".order-id .id { font-size: 18px; font-weight: bold; color: #667eea; }");
        sb.AppendLine(
            "h2 { font-size: 18px; color: #333; margin-bottom: 20px; border-bottom: 2px solid #667eea; padding-bottom: 10px; }");
        sb.AppendLine(".products-table { width: 100%; border-collapse: collapse; margin-bottom: 30px; }");
        sb.AppendLine(
            ".products-table th { background-color: #f5f5f5; padding: 12px; text-align: left; font-weight: 600; border-bottom: 2px solid #ddd; }");
        sb.AppendLine(".products-table td { padding: 12px; border-bottom: 1px solid #eee; }");
        sb.AppendLine(".products-table .price { text-align: right; font-weight: 500; }");
        sb.AppendLine(
            ".summary { background-color: #f9f9f9; padding: 20px; border-radius: 4px; margin-bottom: 30px; }");
        sb.AppendLine(
            ".summary-row { display: flex; justify-content: space-between; padding: 8px 0; font-size: 14px; }");
        sb.AppendLine(
            ".summary-row.total { font-size: 18px; font-weight: bold; color: #667eea; border-top: 2px solid #ddd; padding-top: 12px; }");
        sb.AppendLine(
            ".status-badge { display: inline-block; background-color: #d4edda; color: #155724; padding: 8px 16px; border-radius: 20px; font-size: 12px; font-weight: 600; margin-bottom: 20px; }");
        sb.AppendLine(
            ".info-section { background-color: #e7f3ff; padding: 15px; border-left: 4px solid #2196F3; border-radius: 4px; margin-bottom: 20px; font-size: 14px; color: #555; }");
        sb.AppendLine(
            ".footer { background-color: #f5f5f5; padding: 30px 20px; text-align: center; border-top: 1px solid #ddd; }");
        sb.AppendLine(".footer p { font-size: 12px; color: #999; margin-bottom: 10px; }");
        sb.AppendLine(
            ".cta-button { display: inline-block; background-color: #667eea; color: white; padding: 12px 30px; text-decoration: none; border-radius: 4px; font-weight: 600; margin: 20px 0; }");
        sb.AppendLine(".cta-button:hover { background-color: #764ba2; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<div class=\"email-container\">");

        // Header
        sb.AppendLine("<div class=\"header\">");
        sb.AppendLine("<h1>✓ Order Confirmed</h1>");
        sb.AppendLine("<p>Your order has been received successfully</p>");
        sb.AppendLine("</div>");

        // Main Content
        sb.AppendLine("<div class=\"content\">");

        // Status Badge
        sb.AppendLine("<span class=\"status-badge\">STATUS: CONFIRMED</span>");

        // Order ID
        sb.AppendLine("<div class=\"order-id\">");
        sb.AppendLine($"<p>Order ID: <span class=\"id\">{order.Id.Value:N}</span></p>");
        sb.AppendLine($"<p>Order Date: {DateTime.UtcNow:MMMM dd, yyyy 'at' hh:mm tt}</p>");
        sb.AppendLine("</div>");

        // Order Items
        sb.AppendLine("<h2>Order Items</h2>");
        sb.AppendLine("<table class=\"products-table\">");
        sb.AppendLine(
            "<thead><tr><th>Product</th><th>Qty</th><th class=\"price\">Price</th><th class=\"price\">Total</th></tr></thead>");
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
        sb.AppendLine($"<span>Subtotal:</span><span>{order.TotalPrice():C}</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"summary-row\">");
        sb.AppendLine("<span>Shipping:</span><span>FREE</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"summary-row\">");
        sb.AppendLine("<span>Tax:</span><span>Included</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"summary-row total\">");
        sb.AppendLine($"<span>Total:</span><span>{order.TotalPrice():C}</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("</div>");

        // Next Steps
        sb.AppendLine("<div class=\"info-section\">");
        sb.AppendLine("<strong>What's Next?</strong><br>");
        sb.AppendLine(
            "We're preparing your order for shipment. You'll receive a tracking number via email as soon as your package ships.");
        sb.AppendLine("</div>");

        // CTA
        sb.AppendLine("<center>");
        sb.AppendLine("<a href=\"#\" class=\"cta-button\">Track Your Order</a>");
        sb.AppendLine("</center>");

        sb.AppendLine("</div>");

        // Footer
        sb.AppendLine("<div class=\"footer\">");
        sb.AppendLine("<p>Questions? Contact us at support@shoply.com</p>");
        sb.AppendLine("<p>&copy; 2026 Shoply. All rights reserved.</p>");
        sb.AppendLine(
            "<p><a href=\"#\" style=\"color: #667eea; text-decoration: none;\">Unsubscribe</a> | <a href=\"#\" style=\"color: #667eea; text-decoration: none;\">Privacy Policy</a></p>");
        sb.AppendLine("</div>");

        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }
}