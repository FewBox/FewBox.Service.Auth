[![Build Status](https://travis-ci.com/FewBox/FewBox.Service.Auth.svg?branch=master)](https://travis-ci.com/FewBox/FewBox.Service.Auth)


# Registry
Usage
On QTS Container Station:

Create application registry by [Create Container] → [Create Registry].
Add Registry to searching list by [Preferences] → [Registry] → [Add] that URL is https://NAS_IP:6088 and check Trust SSL.
Others:

Add certificate to your Docker trusty list:
$ mkdir -p /etc/docker/certs.d/NAS_IP:6088
$ scp admin@NAS_IP:/etc/docker/tls/ca.pem /etc/docker/certs.d/NAS_IP:6088/ca.crt
Push an image to the Registry:
$ docker pull busybox:latest
$ docker tag busybox NAS_IP:6088/username/busybox
$ docker push NAS_IP:6088/username/busybox

Copy the ca.crt file to the Windows 10 machine on which you run the Docker client.
Right-click the ca.crt file and select Install Certificate.
Follow the prompts of the wizard to install the certificate.
Restart the Docker daemon:
    Click the up arrow in the task bar to show running tasks.
    Right-click the Docker icon and select Settings.
    Select Reset and click Restart Docker.

https://NAS_IP:6088/v2/_catalog
https://NAS_IP:6088/v2/{repname}/tags/list

# MySQL
https://NAS_IP:49154

主机	Container	通讯协议	
49174 3306 TCP
49173 33060 TCP

# RibbitMQ
https://NAS_IP:49154 Web 15672
https://NAS_IP:49156 Client 5672

主机	Container	通讯协议	
49155 15671 TCP
49154 15672 TCP
49153 25672 TCP
49158 4369 TCP
49157 5671 TCP
49156 5672 TCP

