# azuread-password-resetter

To run the sample locally you need to:
* Register an application in Azure AD
  * Add the following API Permissions:
    * Directory.Read.All
    * User.Read.All
  * Grant Admin Consent
* Assign the **User Administrator** role to the Service Principal of your application
* Create an **[Azure Communication Service](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/create-communication-resource?pivots=platform-azp&tabs=windows)** 
* Create an **[Azure Email Communication Service Resource](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/create-email-communication-resource)** and [connect the domain to the Communication Service](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/connect-email-communication-resource)
* Add the following rows to the **'Values'** section in the *local.appsettings.json* file:

```
    "TenantId": "[AZURE AD TENANT ID] - (ex: cb37aa6f-c9fr-4287-9074-cf11119ed0ed)",
    "ClientId": "[AZURE AD APP CLIENT ID] - (ex. 512333fd-71b1-4194-ab4b-7d14e1da8765)",
    "ClientSecret": "[AZURE AD APP SECRET] - (ex. FhE8Q~7SPhzJsSDFP-9BSan__9raKHQ02PsS2bS4)",

    "AzureADGroupName": "[NAME OF THE USERS GROUP] - (ex. Test Users)",

    "CommunicationServiceConnectionString": "[YOUR COMMUNICATION SERVICES CONNECTION STRING HERE]",
    "SenderAddress": "[YOUR COMMUNICATION SERVICES SENDER ADDRESS HERE] - (ex. DoNotReply@5745eed7-f9ca-48b8-4657-e5193e0550d4.azurecomm.net)",
    "RecipientAddress": "[YOUR RECIPIENT ADDRESS HERE] - (ex. user@contoso.com)"
```

* Put your own values instead of placeholders
* Press F5 in Visual Studio to run the Functions locally (You need the **Azure Functions Tools**. To add Azure Function Tools, include the Azure development workload in your Visual Studio installation)
* To test the function, there is an HttpTrigger function which can be triggered using the *RestClient.http* file in Visual Studio, or using your preferred http client (like Postman) to make a GET call to the local endpoint: http://localhost:7212/api/HttpTriggerResetPasswords