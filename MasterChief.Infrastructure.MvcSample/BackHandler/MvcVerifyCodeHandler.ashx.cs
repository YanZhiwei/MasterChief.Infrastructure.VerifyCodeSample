using System.Web;
using MasterChief.DotNet.Infrastructure.VerifyCode;
using MasterChief.DotNet.Infrastructure.VerifyCode.ValidateCode;

namespace MasterChief.Infrastructure.MvcSample.BackHandler
{
    /// <summary>
    ///     处理生成Mvc 程序验证码
    /// </summary>
    public sealed class MvcVerifyCodeHandler : VerifyCodeHandler
    {
        public override void OnValidateCodeCreated(HttpContext context, string validateCode)
        {
            context.Session["validateCode"] = validateCode;
        }

        public override byte[] CreateValidateCode(string style)
        {
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