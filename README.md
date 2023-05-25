# azuread-password-resetter

To run the sample locally you need to:
* Create an **Azure Communication Service**
* Create an **Azure Email Communication Service** and connect the domain to the Communication Service
* Add the following rows to the **'Values'** section in the *local.appsettings.json* file:

```
    "TenantId": "[AZURE AD TENANT ID]",
    "ClientId": "[AZURE AD APP CLIENT ID]",
    "ClientSecret": "[AZURE AD APP SECRET]",

    "AzureADGroupName": "[NAME OF THE USERS GROUP]",

    "CommunicationServiceConnectionString": "[YOUR COMMUNICATION SERVICES CONNECTION STRING HERE]",
    "SenderAddress": "[YOUR COMMUNICATION SERVICES SENDER ADDRESS HERE]",
    "RecipientAddress": "[YOUR RECIPIENT ADDRESS HERE]"
```