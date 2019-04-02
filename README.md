# 构建验证码示例
> 使用说明
>
> 1. 支持WebForm以及Asp.Net Mvc构建验证码
>
> 2. 项目源码：[**MasterChief.DotNet.Infrastructure.VerifyCode**](https://github.com/YanZhiwei/MasterChief/tree/master/MasterChief.DotNet.Infrastructure.VerifyCode)
>
> 3. Nuget：Install-Package MasterChief.DotNet.Infrastructure.VerifyCode
>
> 4. 欢迎Star，欢迎PR；
>
>    

#### 如何使用

1. 自定义验证码样式，只需要实现ValidateCodeType抽象类即可

   ```c#
   /// <summary>
    ///     图片验证码抽象类
    /// </summary>
    public abstract class ValidateCodeType
    {
        #region Methods
    
        /// <summary>
        ///     创建验证码抽象方法
        /// </summary>
        /// <param name="validataCode">验证code</param>
        /// <returns>Byte数组</returns>
        public abstract byte[] CreateImage(out string validataCode);
    
        #endregion Methods
    
        #region Constructors
    
        #endregion Constructors
    
        #region Properties
    
        /// <summary>
        ///     验证码类型名称
        /// </summary>
        public abstract string Name { get; }
    
        /// <summary>
        ///     验证码Tooltip
        /// </summary>
        public virtual string Tip => "请输入图片中的字符";
    
        /// <summary>
        ///     类型名称
        /// </summary>
        public string Type => GetType().Name;
    
        #endregion Properties
    }
   ```

2. 在WebForm使用说明

   1. 新建一般处理程序

      ```c#
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
      ```

   2. 前端页面调用

      ```c#
      <body>
          <form runat="server">
          <div class="row">
              <div class="col-md-8">
                  <section id="loginForm">
                      <div class="form-horizontal">
                          <h4>Use a local account to log in.</h4>
                          <hr />
                          <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                              <p class="text-danger">
                                  <asp:Literal runat="server" ID="FailureText" />
                              </p>
                          </asp:PlaceHolder>
                          <div class="form-group">
                              <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 control-label">Email</asp:Label>
                              <div class="col-md-10">
                                  <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                                  <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                                      CssClass="text-danger" ErrorMessage="The email field is required." />
                              </div>
                          </div>
                          <div class="form-group">
                              <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
                              <div class="col-md-10">
                                  <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                                  <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The password field is required." />
                              </div>
                          </div>
                          <div class="form-group">
                              <%--  <asp:Image ID="Image1" runat="server" CssClass="col-md-2 control-label" ImageUrl="BackHandler/WebFormVerifyCodeHandler.ashx" />--%>
                              <img alt="看不清，换一张" class="col-md-2 control-label" src="BackHandler/WebFormVerifyCodeHandler.ashx" onclick="this.src = 'BackHandler/WebFormVerifyCodeHandler.ashx?style=type1&ver=' + Math.random()" />
                              <div class="col-md-10">
                                  <asp:TextBox runat="server" ID="VerifyCode" CssClass="form-control" />
                                  <asp:RequiredFieldValidator runat="server" ControlToValidate="VerifyCode" CssClass="text-danger" ErrorMessage="The VerifyCode field is required." />
                              </div>
                          </div>
                          <div class="form-group">
                              <div class="col-md-offset-2 col-md-10">
                                  <div class="checkbox">
                                      <asp:CheckBox runat="server" ID="RememberMe" />
                                      <asp:Label runat="server" AssociatedControlID="RememberMe">Remember me?</asp:Label>
                                  </div>
                              </div>
                          </div>
                          <div class="form-group">
                              <div class="col-md-offset-2 col-md-10">
                                  <asp:Button runat="server" Text="Log in" CssClass="btn btn-default" OnClick="Login_Click" />
                              </div>
                          </div>
                      </div>
                      <p>
                          <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register as a new user</asp:HyperLink>
                      </p>
                      <p>
                          <%-- Enable this once you have account confirmation enabled for password reset functionality --%>
                          <asp:HyperLink runat="server" ID="ForgotPasswordHyperLink" ViewStateMode="Disabled">Forgot your password?</asp:HyperLink>
                      </p>
                  </section>
              </div>
       
              <div class="col-md-4">
              </div>
          </div>
          </form>
      </body>
      ```

   3. 后端页面使用

      ```c#
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
      ```

   4. 运行效果

      ![](https://kxg9jg.dm.files.1drv.com/y4mp3fTuAQemg0iphGfD70f-gLv7wqvYfmkGWvYLOOSlwHynJX0kBSEAyWgXGR32fqFD1M1nRFIN6MX24ruHqAfwPHtMxWdBDE93mjL2p3jeQoB8KxpeEpg9TrnqDMmVH0eWgE-NLNspi0W8TMPEl1ab4NQyJlGZmv2lbJMKAxRQa-HgWPELumrBNA0Hfd6kzD8LeNKAy6uNHv9gk4a3824FQ?width=460&height=319&cropmode=none)

3. 在Asp.Net Mvc使用说明

   1. 新建MvcVerifyCodeHandler，并实现抽象类VerifyCodeHandler

      ```c#
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
      ```

   2. 在Controller处理验证码生成

      ```c#
      /// <summary>
      ///     生成验证码
      /// </summary>
      /// <param name="style">验证码样式</param>
      /// <returns>ActionResult</returns>
      [AllowAnonymous]
      public ActionResult CreateVerifyCode(string style)
      {
          VerifyCodeHandler verifyCodeHandler = new MvcVerifyCodeHandler();
          var buffer = verifyCodeHandler.CreateValidateCode(style);
          return File(buffer, MimeTypes.ImageGif);
      }
      ```

   3. 前端页面调用

      ```c#
      @model MasterChief.Infrastructure.MvcSample.Models.LoginViewModel
      @{
          ViewBag.Title = "Login";
      }
       
      <h2>@ViewBag.Title.</h2>
      <div class="row">
          <div class=" col-md-8">
       
              <section id="loginForm">
                  @using (Html.BeginForm("Login", "Account", new { ViewBag.ReturnUrl }, FormMethod.Post, new { @class = "form-horizontal", role = "form" }))
                  {
                      @Html.AntiForgeryToken()
                      <h4>Use a local account to log in.</h4>
                      <hr />
                      @Html.ValidationSummary(true, "", new { @class = "text-danger" })
                      <div class="form-group">
                          @Html.LabelFor(m => m.Email, new { @class = "col-md-2 control-label" })
                          <div class="col-md-10">
                              @Html.TextBoxFor(m => m.Email, new { @class = "form-control" })
                              @Html.ValidationMessageFor(m => m.Email, "", new { @class = "text-danger" })
                          </div>
                      </div>
                      <div class="form-group">
                          @Html.LabelFor(m => m.Password, new { @class = "col-md-2 control-label" })
                          <div class="col-md-10">
                              @Html.PasswordFor(m => m.Password, new { @class = "form-control" })
                              @Html.ValidationMessageFor(m => m.Password, "", new { @class = "text-danger" })
                          </div>
                      </div>
                      <div class="form-group">
                          <img id="valiCode" style="cursor: pointer;" class="col-md-2 control-label" src="~/Account/CreateVerifyCode" alt="验证码" />
                          <div class="col-md-10">
                              @Html.TextBoxFor(m => m.VerifyCode, new { @class = "form-control" })
                              @Html.ValidationMessageFor(m => m.VerifyCode, "", new { @class = "text-danger" })
                          </div>
                      </div>
                      <div class="form-group">
                          <div class="col-md-offset-2 col-md-10">
                              <div class="checkbox">
                                  @Html.CheckBoxFor(m => m.RememberMe)
                                  @Html.LabelFor(m => m.RememberMe)
                              </div>
                          </div>
                      </div>
                      <div class="form-group">
                          <div class="col-md-offset-2 col-md-10">
                              <input type="submit" value="Log in" class="btn btn-default" />
                          </div>
                      </div>
                  }
              </section>
          </div>
          <div class="col-md-4">
              @*<section id="socialLoginForm">
                      @Html.Partial("_ExternalLoginsListPartial", new ExternalLoginListViewModel {ReturnUrl = ViewBag.ReturnUrl})
                  </section>*@
          </div>
      </div>
       
      @section Scripts{
          <script type="text/javascript">
              $(function () {
                  $("#valiCode").bind("click", function () {
                      this.src = "CreateVerifyCode?style=type1&time=" + (new Date()).getTime();
                  });
              });
          </script>
      }
      ```

   4. 后端代码使用

      ```c#
      [HttpPost]
      [AllowAnonymous]
      [ValidateAntiForgeryToken]
      public ActionResult Login(LoginViewModel model, string returnUrl)
      {
          if (!ModelState.IsValid) return View(model);
          if (string.Compare(Session["validateCode"].ToString(), model.VerifyCode,
                  StringComparison.OrdinalIgnoreCase) != 0)
              ModelState.AddModelError("VerifyCode", "验证码验证不通过.");
          else
              return RedirectToAction("Index", "Home");
       
          return View();
      }
      ```

   5. 运行效果

      ![](https://kxiigq.dm.files.1drv.com/y4mvVZIBjafJ_RefU6KVq7oUwwMZwWBbQRZIFz4JXWhZcUTvNiGHsbg3l7wnOkHplqcCYtjSrKhawTOpjmK7G9nLkV2qwYHosmjcKWDaLuVzfUSX_iQ7ycC6Hr4j52XBS4EyjItJ2WODGnp-dlJfdMmaWOh627YqHF48zRnCIEA4wCFCuleYndvWnJgPZqmT5sEbd8a3-MHret8ITSfqiThBw?width=444&height=407&cropmode=none)