pipeline {
    agent any
    
    environment{
        DEPLOY_PATH = "D:\\WebData\\tlg-mes-api.isdcorp.vn"
        APPLICATION = "tlg-mes-api.isdcorp.vn"
        SLN = "TLG_API.sln"
	CSPROJ = "src/Services/MES/MES.API/MES.API.csproj"
    }

    stages {

        stage('Build project') {
            steps {
                powershell '''
                    dotnet build ${env:SLN}
                '''
            }
        }
        
        stage('Publish project') {
            steps {
                 powershell '''
                    dotnet publish ${env:CSPROJ} -o "Publish" /p:Environment=UAT -c Release
                '''
            }
        }
        
        
        stage('Deploy to IIS') {
            steps {
                 powershell '''
                    $deploymentPool = "${env:APPLICATION}"
                    
                    Stop-WebappPool -Name $deploymentPool
                    Stop-Website -Name $deploymentPool
                    
                    Start-sleep -Seconds 5
                    
                    robocopy "Publish" "${env:DEPLOY_PATH}" /e
                    
                    Start-WebappPool -Name $deploymentPool
                    Start-Website -Name $deploymentPool
                    
                    exit 0
                '''
            }
            post {
                failure {
                 echo 'Deploy to IIS FAILURE'   
                }
            }
        }
        
    }
}
