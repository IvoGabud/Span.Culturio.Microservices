#!/bin/bash

echo "=== Deploying Culturio Microservices to Kubernetes ==="

echo "Starting Minikube..."
minikube start --driver=docker

echo "Enabling Ingress addon..."
minikube addons enable ingress

echo "Setting up Minikube Docker environment..."
eval $(minikube docker-env)

echo "Building Docker images..."
docker build -t culturio-auth:latest ./Span.Culturio.Auth
docker build -t culturio-users:latest ./Span.Culturio.Users
docker build -t culturio-cultureobjects:latest ./Span.Culturio.CultureObjects
docker build -t culturio-packages:latest ./Span.Culturio.Packages
docker build -t culturio-subscriptions:latest ./Span.Culturio.Subscriptions
docker build -t culturio-api-gateway:latest ./Span.Culturio.ApiGateway

echo "Applying Kubernetes manifests..."

kubectl apply -f ./k8s/base/namespace.yaml
kubectl apply -f ./k8s/base/secrets.yaml
kubectl apply -f ./k8s/base/configmap.yaml

kubectl apply -f ./k8s/infrastructure/sqlserver-deployment.yaml
kubectl apply -f ./k8s/infrastructure/sqlserver-service.yaml

echo "Waiting for SQL Server to be ready..."
kubectl wait --for=condition=ready pod -l app=sqlserver -n culturio --timeout=600s

kubectl delete job db-init -n culturio --ignore-not-found=true
kubectl apply -f ./k8s/infrastructure/db-init-job.yaml
kubectl wait --for=condition=complete job/db-init -n culturio --timeout=120s

kubectl apply -f ./k8s/infrastructure/seq-deployment.yaml
kubectl apply -f ./k8s/infrastructure/seq-service.yaml

kubectl apply -f ./k8s/services/auth-deployment.yaml
kubectl apply -f ./k8s/services/auth-service.yaml
kubectl apply -f ./k8s/services/users-deployment.yaml
kubectl apply -f ./k8s/services/users-service.yaml
kubectl apply -f ./k8s/services/cultureobjects-deployment.yaml
kubectl apply -f ./k8s/services/cultureobjects-service.yaml
kubectl apply -f ./k8s/services/packages-deployment.yaml
kubectl apply -f ./k8s/services/packages-service.yaml
kubectl apply -f ./k8s/services/subscriptions-deployment.yaml
kubectl apply -f ./k8s/services/subscriptions-service.yaml

kubectl apply -f ./k8s/gateway/api-gateway-deployment.yaml
kubectl apply -f ./k8s/gateway/api-gateway-service.yaml
kubectl apply -f ./k8s/gateway/ingress.yaml

echo "=== Deployment complete! ==="
echo "Run 'minikube tunnel' to access: http://localhost/swagger"
