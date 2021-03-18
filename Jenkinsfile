library 'jenkins-ptcs-library@3.1.0'

// pod provides common utilies and tools to jenkins-ptcs-library function correctly.
// certain ptcs-library command requires containers (like docker or gcloud.)
podTemplate(label: pod.label,
  containers: pod.templates + [ // This adds all depencies for jenkins-ptcs-library methods to function correctly.
      containerTemplate(name: 'dotnet', image: 'mcr.microsoft.com/dotnet/core/sdk:3.1.100-alpine3.10', ttyEnabled: true, command: '/bin/sh -c', args: 'cat'),
  ]
) {
    node(pod.label) {
      stage('Checkout') {
          checkout scm
      }
      stage('Build') {
        container('dotnet') {
          sh """
            dotnet publish -c Release -o ${workspace}/out/ ./Azure.Container.Updater.csproj
          """
        }
      }
      stage('Deploy') {
        if(env.BRANCH_NAME == "master") {
            withCredentials([string(credentialsId: 'CONTAINER_IMAGE_UPDATER_API_KEY', variable: 'API_KEY')]) {
                toAzureTestEnv {
                    sh "pwsh -command './Deployment/Deploy.ps1 -Tags @{subproject='2026956'} -ResourceGroup pinja-azure-container-updater' -ApiKey '$API_KEY' -PublishFolder '${workspace}/out/'"
                }
            }
        }
      }
    }
  }