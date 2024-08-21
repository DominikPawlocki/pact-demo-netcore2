# pact-demo-netcore2

# UPDATE to .NET8 and PACT 5 #
Time passes, things changes.. I updated this old .Net Core to .Net 8 and _Pact 5.0.0-beta2_ Nuget. Many things changed in pact, however idea stays the same.

## How to ##
1) **Install, configure and run Pact broker** on localhost (or anywhere) from the DockerHub (https://hub.docker.com/r/pactfoundation/pact-broker), using docker compose. There are 3 images there - Postgres db, ngnix and pact-broker itself. There is need to do some simple setup like Postgres password etc.
I have run it on localhost : 
![Alt text](/dockerContainers.JPG?raw=true)
Check if broker is working via browser : 
![Alt text](/pactFront.JPG?raw=true)
2) **Run Consumer tests in VS**
After a run a pact json file is created in folder : *...\pact-demo-netcore2\consumers\Pact.Consumer.MVC\Pact.Consumer.MVC.PactTests\bin\pacts*.
Remember the pact appends to the file (probably cause several concurrent xUnit processes (tests) to be running in parallel), so its worth to delete the file before a new run for new upload.
![text:](/consumer.jpg)
3) **Upload a pact file to Pact broker** 
The Powershell script from there doesn't work anymore with new broker, so I just upload it manually by Postman... First, just CTRL-C all the content of pact json file from step 2 and do a base64 from it (ie via some online converter), then you can just paste it to the request body, increment 'pacticipantVersionNumber', branch, tags, etc and upload by HTTP Post (adjust authorization if you set it on step 1, I didn't)
![Upload](/postmanUpload.jpg?raw=true )
4) **Run tests on Provider side**
![Alt text](/providerNew.jpg?raw=true)

* [PactFoundation GitHub ](https://github.com/pact-foundation)
* [PactFoundation](https://docs.pact.io/)


##
##
##
## ---------------------OLD--------------------------------------- ##
# A demo for Tricity .Net Group on 21/02/2019 #

You can upload your pact's to broker (http://52.157.178.169/) by using upload.ps1 script. 
(\consumers\Pact.Consumer.MVC\Pact.Consumer.MVC.PactTests.\upload.ps1 branch-name version)
![Alt text](/howToUpload.JPG?raw=true )

Build at least consumer solution first (Pact.Consumer.MVC) for create a Pact artifact file. 

Feel free to contact me at pawlockiDmnk@gmail.com

Thx for attending !

CONSUMER:
![Alt text](/consumer.jpg?raw=true )

PROVIDER
![Alt text](/provider.jpg?raw=true)