using System.Text;
using Shoply.WebApi.Common.Infrastructure.Services.Emails.Templates;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EmailTemplates;

internal sealed class CheckoutSessionCompletedEmailTemplate(string userEmail, Order order) : UserEmailTemplate(userEmail)
{
    public override string Subject { get; } = $"Checkout completed - Order #{order.Id}";
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
            ".header { background: linear-gradient(135deg, #1f9d55 0%, #15803d 100%); color: white; padding: 40px 20px; text-align: center; }");
        sb.AppendLine(".header h1 { font-size: 28px; margin-bottom: 10px; }");
        sb.AppendLine(".header p { font-size: 14px; opacity: 0.9; }");
        sb.AppendLine(".content { padding: 40px 20px; }");
        sb.AppendLine(
            ".order-id { background-color: #f0fdf4; padding: 15px; border-left: 4px solid #15803d; margin-bottom: 30px; border-radius: 4px; }");
        sb.AppendLine(".order-id p { font-size: 14px; color: #666; }");
        sb.AppendLine(".order-id .id { font-size: 18px; font-weight: bold; color: #15803d; }");
        sb.AppendLine(
            "h2 { font-size: 18px; color: #333; margin-bottom: 20px; border-bottom: 2px solid #1f9d55; padding-bottom: 10px; }");
        sb.AppendLine(".products-table { width: 100%; border-collapse: collapse; margin-bottom: 30px; }");
        sb.AppendLine(
            ".products-table th { background-color: #f5f5f5; padding: 12px; text-align: left; font-weight: 600; border-bottom: 2px solid #ddd; }");
        sb.AppendLine(".products-table td { padding: 12px; border-bottom: 1px solid #eee; }");
        sb.AppendLine(".products-table .price { text-align: right; font-weight: 500; }");
        sb.AppendLine(
            ".summary { background-color: #f0fdf4; padding: 20px; border-radius: 4px; margin-bottom: 30px; }");
        sb.AppendLine(
            ".summary-row { display: flex; justify-content: space-between; padding: 8px 0; font-size: 14px; }");
        sb.AppendLine(
            ".summary-row.total { font-size: 18px; font-weight: bold; color: #15803d; border-top: 2px solid #ddd; padding-top: 12px; }");
        sb.AppendLine(
            ".status-badge { display: inline-block; background-color: #d1fae5; color: #065f46; padding: 8px 16px; border-radius: 20px; font-size: 12px; font-weight: 600; margin-bottom: 20px; }");
        sb.AppendLine(
            ".info-section { background-color: #e0f2fe; padding: 15px; border-left: 4px solid #0284c7; border-radius: 4px; margin-bottom: 20px; font-size: 14px; color: #0c4a6e; }");
        sb.AppendLine(
            ".payment-section { background-color: #fff7ed; padding: 15px; border-left: 4px solid #ea580c; border-radius: 4px; margin-bottom: 20px; font-size: 14px; color: #9a3412; }");
        sb.AppendLine(
            ".footer { background-color: #f5f5f5; padding: 30px 20px; text-align: center; border-top: 1px solid #ddd; }");
        sb.AppendLine(".footer p { font-size: 12px; color: #999; margin-bottom: 10px; }");
        sb.AppendLine(
            ".cta-button { display: inline-block; background-color: #15803d; color: white; padding: 12px 30px; text-decoration: none; border-radius: 4px; font-weight: 600; margin: 20px 0; }");
        sb.AppendLine(".cta-button:hover { background-color: #166534; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<div class=\"email-container\">");

        sb.AppendLine("<div class=\"header\">");
        sb.AppendLine("<h1>Payment Confirmed</h1>");
        sb.AppendLine("<p>Your checkout session was completed successfully</p>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class=\"content\">");

        sb.AppendLine("<span class=\"status-badge\">STATUS: CHECKOUT COMPLETED</span>");

        sb.AppendLine("<div class=\"order-id\">");
        sb.AppendLine($"<p>Order ID: <span class=\"id\">{order.Id.Value:N}</span></p>");
        sb.AppendLine($"<p>Checkout Date: {DateTime.UtcNow:MMMM dd, yyyy 'at' hh:mm tt}</p>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class=\"payment-section\">");
        sb.AppendLine("<strong>Payment Status</strong><br>");
        sb.AppendLine("We have received your payment and your order is now being prepared for shipment.");
        sb.AppendLine("</div>");

        sb.AppendLine("<h2>Order Items</h2>");
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

        sb.AppendLine("<div class=\"summary\">");
        sb.AppendLine("<div class=\"summary-row\">");
        sb.AppendLine($"<span>Subtotal:</span><span>{order.TotalPrice():C}</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"summary-row\">");
        sb.AppendLine("<span>Shipping:</span><span>FREE</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"summary-row\">");
        sb.AppendLine("<span>Payment:</span><span>Successful</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"summary-row total\">");
        sb.AppendLine($"<span>Total Paid:</span><span>{order.TotalPrice():C}</span>");
        sb.AppendLine("</div>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class=\"info-section\">");
        sb.AppendLine("<strong>What Happens Next?</strong><br>");
        sb.AppendLine("Our team is preparing your package. You will receive another email as soon as your order ships.");
        sb.AppendLine("</div>");

        sb.AppendLine("<center>");
        sb.AppendLine("<a href=\"#\" class=\"cta-button\">View Order Details</a>");
        sb.AppendLine("</center>");

        sb.AppendLine("</div>");

        sb.AppendLine("<div class=\"footer\">");
        sb.AppendLine("<p>Need help with your order? Contact us at support@shoply.com</p>");
        sb.AppendLine("<p>&copy; 2026 Shoply. All rights reserved.</p>");
        sb.AppendLine(
            "<p><a href=\"#\" style=\"color: #15803d; text-decoration: none;\">Unsubscribe</a> | <a href=\"#\" style=\"color: #15803d; text-decoration: none;\">Privacy Policy</a></p>");
        sb.AppendLine("</div>");

        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }
}