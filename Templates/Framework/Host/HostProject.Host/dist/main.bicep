param location string = 'eastus'
param clusterName string = 'myAKSCluster'
param dnsPrefix string = 'myAKSDnsPrefix'
param subnetId string
param imageName string
@secure()
param clientId string
@secure()
param clientSecret string


resource aci 'Microsoft.ContainerInstance/containerGroups@2021-03-01' = {
  name: '${clusterName}-aci'
  location: location
  properties: {
    containers: [
      {
        name: '${clusterName}-container'
        properties: {
          image: imageName
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 1
            }
          }
        }
      }
    ]
    osType: 'Linux'
    ipAddress: {
      type: 'Private'
      ports: [{port: 8080, protocol: 'https'}]
    }
  }
}

resource aksCluster 'Microsoft.ContainerService/managedClusters@2022-11-01' = {
  name: '${clusterName}-aks'
  location: location
  properties: {
    dnsPrefix: dnsPrefix
    kubernetesVersion: '1.19.11'
    networkProfile: {
      networkPlugin: 'azure'
      serviceCidr: '172.20.0.0/16'
      dnsServiceIP: '172.20.0.10'
      dockerBridgeCidr: '172.17.0.1/16'
      loadBalancerSku: 'standard'
    }
    agentPoolProfiles: [
      {
        name: 'default'
        count: 3
        vmSize: 'Standard_D2s_v3'
        osType: 'Linux'
        vnetSubnetID: subnetId
        maxPods: 110
      }
    ]
    servicePrincipalProfile: {
      clientId: clientId
      secret: clientSecret
    }
  }
}
