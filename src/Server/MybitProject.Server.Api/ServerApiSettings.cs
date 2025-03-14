﻿using AdsPush.Abstraction.Settings;

namespace MybitProject.Server.Api;

public partial class ServerApiSettings : SharedSettings
{
    /// <summary>
    /// It can also be configured using: dotnet user-secrets set 'DataProtectionCertificatePassword' '@nyPassw0rd'
    /// </summary>
    [Required]
    public string DataProtectionCertificatePassword { get; set; } = default!;

    [Required]
    public AppIdentityOptions Identity { get; set; } = default!;

    [Required]
    public EmailOptions Email { get; set; } = default!;

    public SmsOptions? Sms { get; set; }

    [Required]
    public string UserProfileImagesDir { get; set; } = default!;


    public AdsPushVapidSettings? AdsPushVapid { get; set; }

    public AdsPushFirebaseSettings? AdsPushFirebase { get; set; }

    public AdsPushAPNSSettings? AdsPushAPNS { get; set; }

    public ForwardedHeadersOptions? ForwardedHeaders { get; set; }

    public CorsOptions? Cors { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResults = base.Validate(validationContext).ToList();

        if (Identity is null)
            throw new InvalidOperationException("Identity configuration is required.");
        if (Email is null)
            throw new InvalidOperationException("Email configuration is required.");

        Validator.TryValidateObject(Identity, new ValidationContext(Identity), validationResults, true);
        Validator.TryValidateObject(Email, new ValidationContext(Email), validationResults, true);
        if (Sms is not null)
        {
            Validator.TryValidateObject(Sms, new ValidationContext(Sms), validationResults, true);
        }
        if (AdsPushVapid is not null)
        {
            Validator.TryValidateObject(AdsPushVapid, new ValidationContext(AdsPushVapid), validationResults, true);
        }
        if (ForwardedHeaders is not null)
        {
            Validator.TryValidateObject(ForwardedHeaders, new ValidationContext(ForwardedHeaders), validationResults, true);
        }

        if (AppEnvironment.IsDev() is false)
        {
            if (DataProtectionCertificatePassword is "P@ssw0rdP@ssw0rd")
            {
                throw new InvalidOperationException(@"The default test certificate is still in use. Please replace it with a new one by running the 'dotnet dev-certs https --export-path DataProtectionCertificate.pfx --password @nyPassw0rd'
command in the Server.Api's project's folder and replace P@ssw0rdP@ssw0rd with the new password.");
            }


            if (AdsPushVapid?.PrivateKey is "dMIR1ICj-lDWYZ-ZYCwXKyC2ShYayYYkEL-oOPnpq9c" || AdsPushVapid?.Subject is "mailto:test@bitplatform.dev")
            {
                throw new InvalidOperationException("The AdsPushVapid's PrivateKey and Subject are not set. Please set them in the server's appsettings.json file.");
            }
        }

        return validationResults;
    }
}

public partial class AppIdentityOptions : IdentityOptions
{
    /// <summary>
    /// BearerTokenExpiration used as JWT's expiration claim, access token's expires in and cookie's max age.
    /// </summary>
    public TimeSpan BearerTokenExpiration { get; set; }
    public TimeSpan RefreshTokenExpiration { get; set; }

    [Required]
    public string Issuer { get; set; } = default!;

    [Required]
    public string Audience { get; set; } = default!;

    /// <summary>
    /// To either confirm and/or change email
    /// </summary>
    public TimeSpan EmailTokenLifetime { get; set; }
    /// <summary>
    /// To either confirm and/or change phone number
    /// </summary>
    public TimeSpan PhoneNumberTokenLifetime { get; set; }
    public TimeSpan ResetPasswordTokenLifetime { get; set; }
    public TimeSpan TwoFactorTokenLifetime { get; set; }

    /// <summary>
    /// To sign in with either Otp or magic link.
    /// </summary>
    public TimeSpan OtpTokenLifetime { get; set; }

    /// <summary>
    /// <inheritdoc cref="AuthPolicies.PRIVILEGED_ACCESS"/>
    /// </summary>
    public int MaxConcurrentPrivilegedSessions { get; set; }
}

public partial class EmailOptions
{
    [Required]
    public string Host { get; set; } = default!;
    /// <summary>
    /// If true, the web app tries to store emails as .eml file in the App_Data/sent-emails folder instead of sending them using smtp server (recommended for testing purposes only).
    /// </summary>
    public bool UseLocalFolderForEmails => Host is "LocalFolder";

    [Range(1, 65535)]
    public int Port { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }

    [Required]
    public string DefaultFromEmail { get; set; } = default!;
    public bool HasCredential => (string.IsNullOrEmpty(UserName) is false) && (string.IsNullOrEmpty(Password) is false);
}

public partial class SmsOptions
{
    public string? FromPhoneNumber { get; set; }
    public string? TwilioAccountSid { get; set; }
    public string? TwilioAutoToken { get; set; }

    public bool Configured => string.IsNullOrEmpty(FromPhoneNumber) is false &&
                              string.IsNullOrEmpty(TwilioAccountSid) is false &&
                              string.IsNullOrEmpty(TwilioAutoToken) is false;
}

public class CorsOptions
{
    public string[] AllowedOrigins { get; set; } = [];
}
