using System;
using System.Web.UI;

namespace MasterChief.Infrastructure.WbSample
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                var verifyCode = VerifyCode.Text.Trim();
                if (string.Compare(Session["validateCode"].ToString(), verifyCode,
                        StringComparison.OrdinalIgnoreCase) != 0)
                {
                    FailureText.Text = "验证码验证不通过.";
                    ErrorMessage.Visible = true;
                }
                else
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }
    }
}