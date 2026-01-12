# Email Notification Setup

This workflow sends email notifications when the CI pipeline fails. The email is sent to:
- `dedejolta@gmail.com` (always)
- The email of the user who pushed the commit (if available)

## Required GitHub Secrets

To enable email notifications, you need to configure the following secrets in your GitHub repository:

1. Go to your repository on GitHub
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret** and add the following secrets:

### Required Secrets

- **`SMTP_SERVER`** - Your SMTP server address
  - Example: `smtp.gmail.com` (for Gmail)
  - Example: `smtp.office365.com` (for Outlook)
  - Example: `smtp.mailgun.org` (for Mailgun)

- **`SMTP_PORT`** - SMTP server port
  - Common values: `587` (TLS), `465` (SSL), `25` (unencrypted, not recommended)

- **`SMTP_USERNAME`** - Your SMTP username/email
  - Example: `your-email@gmail.com`

- **`SMTP_PASSWORD`** - Your SMTP password or app-specific password
  - For Gmail: Use an [App Password](https://support.google.com/accounts/answer/185833) (not your regular password)
  - For other providers: Use your SMTP password or API key

- **`SMTP_FROM`** - The "From" email address
  - Example: `noreply@yourdomain.com` or `your-email@gmail.com`
  - Should match your SMTP_USERNAME for most providers

## Common SMTP Providers

### Gmail
```
SMTP_SERVER: smtp.gmail.com
SMTP_PORT: 587
SMTP_USERNAME: your-email@gmail.com
SMTP_PASSWORD: [App Password - see below]
SMTP_FROM: your-email@gmail.com
```

**Note:** For Gmail, you need to:
1. Enable 2-factor authentication
2. Generate an [App Password](https://support.google.com/accounts/answer/185833)
3. Use the App Password (16 characters) as `SMTP_PASSWORD`

### Outlook/Office 365
```
SMTP_SERVER: smtp.office365.com
SMTP_PORT: 587
SMTP_USERNAME: your-email@outlook.com
SMTP_PASSWORD: [Your password]
SMTP_FROM: your-email@outlook.com
```

### Mailgun
```
SMTP_SERVER: smtp.mailgun.org
SMTP_PORT: 587
SMTP_USERNAME: postmaster@your-domain.mailgun.org
SMTP_PASSWORD: [Your Mailgun SMTP password]
SMTP_FROM: noreply@yourdomain.com
```

## Testing

After setting up the secrets, you can test the email notification by:
1. Intentionally breaking the build (e.g., add a syntax error)
2. Push the change
3. Wait for the pipeline to fail
4. Check your email inbox

## Troubleshooting

- **Emails not being sent**: Check that all secrets are correctly set and the SMTP credentials are valid
- **Gmail authentication fails**: Make sure you're using an App Password, not your regular password
- **Port issues**: Try port 587 (TLS) first, then 465 (SSL) if your provider requires it
- **Check workflow logs**: The email notification step will show errors if SMTP configuration is incorrect

## Alternative: GitHub Notifications

If you don't want to set up SMTP, GitHub already sends email notifications for workflow failures to:
- Repository owners
- Users who have subscribed to repository notifications

You can enable this in **Settings** → **Notifications** → **Actions**.

