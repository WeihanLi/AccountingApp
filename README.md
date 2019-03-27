# AccountingApp

## Build Status

[![Build Status](https://weihanli.visualstudio.com/Pipelines/_apis/build/status/WeihanLi.ActivityReservation?branchName=dev)](https://weihanli.visualstudio.com/Pipelines/_build/latest?definitionId=7?branchName=dev)

## Intro

This a simple accounting app powered by asp.net core.

## Docker Suppport

### Build docker image

``` bash
docker build -t weihanli/accountingapp .
```

### Run in container

``` bash
docker run -P --rm --name accountingapp-demo weihanli/accountingapp
```

### Deploy in k8s

``` bash
kubectl apply -f k8s-deployment.yaml

kubectl expose deployment accountingapp-deployment --type=LoadBalancer --port=8092 --target-port=80 --name accountingapp-deployment
```

## Contact

Contact me: <weihanli@outlook.com>