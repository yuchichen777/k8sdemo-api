$version = "v8"

Write-Host "Build Docker image: k8sdemo-api:$version"
docker build -t k8sdemo-api:$version .

Write-Host "Load image to Minikube"
minikube image load k8sdemo-api:$version

Write-Host "Helm upgrade"
helm upgrade k8sdemo-release ./k8sdemo-chart `
  --set image.repository=k8sdemo-api `
  --set image.tag=$version `
  --set image.pullPolicy=Never

Write-Host "Wait rollout"
kubectl rollout status deployment/k8sdemo-release-k8sdemo-chart

Write-Host "Pods"
kubectl get pods