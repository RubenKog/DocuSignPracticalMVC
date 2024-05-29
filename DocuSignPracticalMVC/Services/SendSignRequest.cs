using DocuSign.eSign.Client;
using DocuSignPracticalMVC.Services.Email;
using DocuSignPracticalMVC.Services.JWAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;
using static DocuSign.eSign.Client.Auth.OAuth;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DocuSignPracticalMVC.Services;

public class SendSignRequest
{
    static readonly string DevCenterPage = "https://developers.docusign.com/platform/auth/consent";
    public void DigitalSignature(string signEmail , string signName)
    {
        Console.ForegroundColor = ConsoleColor.White;
        OAuthToken accessToken = null;
        try
        {
            accessToken = JwtAuth.AuthenticateWithJwt("ESignature", "c58dd321-880a-4732-ba88-ff8971488bba", "09df5276-9822-4479-9649-5505ab0afa82",
                                                        "account-d.docusign.com", DsHelper.ReadFileContent("C:\\Users\\ruben\\source\\repos\\Gradproef\\DocuSignPracticalMVC\\DocuSignPracticalMVC\\private.key"));
        }
        catch (ApiException apiExp)
        {
            // Consent for impersonation must be obtained to use JWT Grant
            if (apiExp.Message.Contains("consent_required"))
            {
                // Caret needed for escaping & in windows URL
                string caret = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    caret = "^";
                }

                // build a URL to provide consent for this Integration Key and this userId
                string url = "https://" + "account-d.docusign.com" + "/oauth/auth?response_type=code" + caret + "&scope=impersonation%20signature" + caret +
                    "&client_id=" + "c58dd321-880a-4732-ba88-ff8971488bba" + caret + "&redirect_uri=" + DevCenterPage;

                string consentRequiredMessage = $"Consent is required - launching browser (URL is {url})";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    consentRequiredMessage = consentRequiredMessage.Replace(caret, "");
                }

                Console.WriteLine(consentRequiredMessage);

                // Start new browser window for login and consent to this app by DocuSign user
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = false });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }

                
            }
        }

        var docuSignClient = new DocuSignClient();
        docuSignClient.SetOAuthBasePath("account-d.docusign.com");
        UserInfo userInfo = docuSignClient.GetUserInfo(accessToken.access_token);
        Account acct = userInfo.Accounts.FirstOrDefault();

        
        string signerEmail = signEmail;        
        string signerName = signName;
        string ccEmail = signEmail;
        string ccName = signerName;
        string docDocx = "C:\\Users\\ruben\\source\\repos\\Gradproef\\DocuSignPracticalMVC\\DocuSignPracticalMVC\\Files\\World_Wide_Corp_salary.docx";
        string docPdf = "C:\\Users\\ruben\\source\\repos\\Gradproef\\DocuSignPracticalMVC\\DocuSignPracticalMVC\\Files\\World_Wide_Corp_lorem.pdf";
        Console.WriteLine("");
        string envelopeId = SigningViaEmail.SendEnvelopeViaEmail(signerEmail, signerName, ccEmail, ccName, accessToken.access_token, acct.BaseUri + "/restapi", acct.AccountId, docDocx, docPdf, "sent");
    }
}
