---
page_type: sample
languages:
  - C#
  - .Net core
products:
  - azure-active-directory
description: "A sample to demonstrate how to validating a sign-up using a C# Azure Function and API connectors"
urlFragment: "active-directory-dotnet-external-identities-api-connector-azure-function-validateb"
---

# External Identities self-service sign-up validation using Node.js Azure Function and API connector.

This sample demonstrates how to use API connectors to customize [self-service sign-up](https://docs.microsoft.com/en-us/azure/active-directory/b2b/self-service-sign-up-overview) of External Identities.

In particular, the sample demonstrates how to:

1. Limit external user sign-ups to only a particular federated Azure Active Directory tenant. In this example, it's a fictitious `fabrikam.com` and `contoso.com`.
1. Validate a user-provided value ('Job Title') against a validation rule.

For the API, an Azure Function HTTP trigger using C# .Net core is implemented (with Basic authentication TBD!!!!!).

## Contents

| File/folder                 | Description                                |
| --------------------------- | ------------------------------------------ |
| [SignUpValidation.cs](SignUpValidation.cs)      | Sample source code for HTTP trigger.       |
| [ResponseContent.cs](ResponseContent.cs)      | HTTP response content.       |
| `.gitignore`                | Define what to ignore at commit time.      |
| `CHANGELOG.md`              | List of changes to the sample.             |
| `CONTRIBUTING.md`           | Guidelines for contributing to the sample. |
| `README.md`                 | This README file.                          |
| `LICENSE`                   | The license for the sample.                |

## Key concepts

External Identities [self-service sign-up](https://docs.microsoft.com/azure/active-directory/b2b/self-service-sign-up-overview) enables you way to create custom experiences for external users like collaborators, partners, and guests to sign-up to to applications in your tenant for easy collaboration.

[API connectors](https://docs.microsoft.com/azure/active-directory/b2b/api-connectors-overview) provide you with a way to further modify and extend sign-up flows by leveraging web APIs. This examples uses an API connector to limit sign-ups to only a specific tenant: fabrikam.com. This is easily modifiable in `index.js`. Further, the API connector is used to perform input validation on 'Job Title' by ensuring a user provides a value of at least 4 characters.

## Prerequisites

Before you get started, make sure you have the following requirements in place:

- An Azure account with an active subscription. [Create an account for free](https://azure.microsoft.com/free/?ref=microsoft.com&utm_source=microsoft.com&utm_medium=docs&utm_campaign=visualstudio).
- A [self-service sign-up user flow](https://docs.microsoft.com/azure/active-directory/b2b/self-service-sign-up-user-flow) in an Azure AD tenant. Only use Azure AD
- [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1). The ASP.NET Core Runtime enables you to run existing web/server applications. 
- [Visual Studio code](https://code.visualstudio.com). A free source-code editor made by Microsoft for Windows, Linux and macOS. Features include support for debugging, syntax highlighting, intelligent code completion, snippets, code refactoring, and embedded Git.
- The [Azure Functions extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions) for Visual Studio Code.

## Setting up your Azure Function

### Steps to run locally

1. Clone the repository

```console
git clone https://github.com/Azure-Samples/active-directory-dotnet-external-identities-api-connector-azure-function-validate
```

1. Navigate to the **Azure extension** in Visual Studio code on the left navigation bar. You should see a 'Local Project' folder representing your local Azure Function.
1. Press **F5** (or use the **Debug > Start Debugging** menu command) to launch the debugger and attach to the Azure Functions host. (This command automatically uses the single debug configuration that Azure Functions created.)
1. The Azure Function extension will automatically generate a few files for local development, install dependencies, and install the Function Core tools if not already present. These tools help with the debugging experience.
1. Output from the Functions Core tools appears in the VS Code **Terminal** panel. Once the host has started, **Alt+click** the local URL shown in the output to open the browser and run the function. You can also see the url of the Local Function by right clicking on the function on the Azure Functions explorer.
1. To redeploy the local instance during testing, just repeat these steps.

### Add authentication

Authentication is stored in environment variables, so they're not stored as part of the repository and should never be stored in checked in code. Read more about the [local.settings.json](https://docs.microsoft.com/azure/azure-functions/functions-run-local?tabs=macos%2Ccsharp%2Cbash#local-settings-file) file.

1. Create a **local.settings.json** file
1. Copy and paste the below code onto the file:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "node",
    "BASIC_AUTH_USERNAME": "<USERNAME>",
    "BASIC_AUTH_PASSWORD": "<PASSWORD>"
  }
}
```

Specify a **Username** and **Password**. This will be what your Azure Function uses to authenticate incoming requests from Azure AD.

### Deploy the application

1. Follow steps of [this](https://docs.microsoft.com/azure/javascript/tutorial-vscode-serverless-node-04) guide #1-7 to deploy your Azure Function to the cloud and get a live API endpoint URL.
1. Once deployed, you'll see a **'Upload settings'** option. Select this. It will upload your environment variables onto the [Application settings](https://docs.microsoft.com/azure/azure-functions/functions-develop-vs-code?tabs=csharp#application-settings-in-azure) of the cloud.

To learn more about Visual Studio Code development for Azure Functions, see [this](https://docs.microsoft.com/azure/azure-functions/functions-develop-vs-code?tabs=csharp#republish-project-files).

## Configure and enable the API connector

Follow the steps outlined in [Add an API connector to a user flow](https://aka.ms/ExtIdAddAPIConnector) to configure and enable the API connector.

### API connector configuration

Your API connector configuration should look like the following:

<img src="images/api-connector-configuration.png" alt="API connector configuration"
    title="API connector configuration" width="400" />

- **Endpoint URL** is the Function URL you copied earlier.
- **Username** and **Password** are the Username and Passwords you defined as environment variables earlier.
- Select **Job Title** as a claim to send in addition to the two preselected claims.

### Enable the API connector

In the **API connector** settings for your user flow, you can select the API connector to be invoked at either step:
![API connector selected](images/api-connector-selected.png)

- **After signing in with an identity provider** - if enabled for this step, the API connector will only allow users with an email ending in `@fabrikam.com`.
- **Before creating the user** - if enabled for this step, the API connector will only allow users with an email ending in `@fabrikam.com` _and_ check whether 'Job Title' is of at least length 4. Note that **Job Title** has to be selected in **User attributes** for the user flow.

## Customizing the Azure Function

This sample provides a quick way to get started using API connectors. By modifying the source code and leveraging all the capabilities of a web API you're used to, you'll be able to accomplish many more complex scenarios.

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit <https://cla.opensource.microsoft.com.>

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
