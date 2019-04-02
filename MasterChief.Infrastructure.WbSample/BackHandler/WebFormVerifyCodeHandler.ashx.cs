using System.Web;
using System.Web.SessionState;
using MasterChief.DotNet.Infrastructure.VerifyCode;
using MasterChief.DotNet.Infrastructure.VerifyCode.ValidateCode;
using MasterChief.DotNet4.Utilities;

namespace MasterChief.Infrastructure.WbSample.BackHandler
{
    /// <summary>
    ///     WebFormVerifyCodeHandler 的摘要说明
    /// </summary>
    public class WebFormVerifyCodeHandler : VerifyCodeHandler, IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var validateType = context.Request.Params["style"];
            var buffer = CreateValidateCode(validateType);
            context.Response.ClearContent();
            context.Response.ContentType = MimeTypes.ImageGif;
            context.Response.BinaryWrite(buffer);
        }

        public bool IsReusable => false;

        public override void OnValidateCodeCreated(HttpContext context, string validateCode)
        {
            context.Session["validateCode"] = validateCode;
        }

        public override byte[] CreateValidateCode(string style)
        {
            style = style?.Trim();
            ValidateCodeType createCode;
            switch (style)
            {
                case "type1":
                    createCode = new ValidateCode_Style1();
                    break;

                default:
                    createCode = new ValidateCode_Style1();
                    break;
            }

            var buffer = createCode.CreateImage(out var validateCode);
            OnValidateCodeCreated(HttpContext.Current, validateCode);
            return buffer;
        }
    }
}