# apache-kafka-csharp

Como produzir e consumir mensagens no Apache Kafka com schema Avro.

# Contexto

Neste exemplo, foi utilizado como base o objeto oferecido pela biblioteca [React Native Background Geolocation](https://transistorsoft.github.io/react-native-background-geolocation/). A situação hipotética consiste em um envio contínuo de dados de geolocalização em tempo real obtidos através de diversos dispositivos móveis. Essa situação caracteriza um Event Stream Processing  (ESP), do português Processamento de Fluxo de Eventos.



# Configuração do ambiente

O ambiente para o funcionamento do Apache Kafka foi configurado usando Docker. Ao executar o comando abaixo, as imagens serão baixadas e o ambiente Docker será automaticamente configurado e hospedado localmente.

`docker-compose up -d`

Após a conclusão da execução, também será disponibilizado acesso as seguintes ferramentas:

- Kafdrop acessível na URL http://localhost:19000
- Confluent Dashboard acessível na URL http://localhost:9021/



## Definição do schema

**Nota:** a definição e build do schema utilizado no projeto já foi feita, porém algumas informações relevantes para essa etapa serão listadas abaixo.

- A definição do schema Avro pode ser encontrada no seguinte link: [Apache Avro™ 1.10.2 Specification](https://avro.apache.org/docs/current/spec.html)
- Para realizar o processo de geração das classes do C# baseadas no schema Avro, será necessário instalar a biblioteca [Confluent.Apache.Avro.AvroGen](https://www.nuget.org/packages/Confluent.Apache.Avro.AvroGen/) de maneira global utilizando o comando a seguir: `dotnet tool install --global Confluent.Apache.Avro.AvroGen`
  - Mais informações em: 
- Feito isso, executar o seguinte comando no diretório que contém o arquivo do schema do tópico (`.asvc`), para para gerar as classes C# que serão utilizadas na Solution: `avrogen -s ./schema.avsc .`



## Criação do tópico e definição do schema

Para configurar o schema corretamente, siga os seguintes passos.

- Acessar o [Confluent Dashboard](http://localhost:19000), depois Cluster 1, Topics, Add a topic.

- Acessar o tópico, clicar em Schema, Value, Set a schema e colar o schema Avro Location que está disponível na pasta **avrogen-schema**. Continuando na guia Schema do tópico, agora clicar em Key e definir como `"string"`.

**Notas:**

- mais informações podem ser encontradas nas seções Create a Topic e Define the Topic Schema do artigo [Add Schema Registry to Kafka in Your Local Docker Environment](https://betterprogramming.pub/adding-schema-registry-to-kafka-in-your-local-docker-environment-49ada28c8a9b)
- O Apache Kafka trata de definir o schema automaticamente caso não exista configuração e uma primeira mensagem seja postada no broker.



## Sobre cacert.pem

O arquivo **cacert.pem** é um pacote de certificados CA utilizados para validar o certificado do servidor. Possui uma lista de autoridades certificadoras que são considerados como signatários aceitáveis do certificado do servidor ([IBM, 2021](https://www.ibm.com/docs/it/security-verify?topic=configuration-cacertpem-file)). O seu uso no projeto é **opicional** bastando adicionar ou remover a variável de ambiente `SSL_CA_LOCATION`.



# Bibliotecas necessárias no NUGET

- Confluent.Kafka

- Confluent.SchemaRegistry

- Confluent.SchemaRegistry.Serdes.Avro

  

# Referências

[Apache Kafka + Kafdrop + Docker Compose: montando rapidamente um ambiente para testes](https://medium.com/azure-na-pratica/apache-kafka-kafdrop-docker-compose-montando-rapidamente-um-ambiente-para-testes-606cc76aa66)

[Add Schema Registry to Kafka in Your Local Docker Environment](https://betterprogramming.pub/adding-schema-registry-to-kafka-in-your-local-docker-environment-49ada28c8a9b)

[Adding Swagger to ASP.NET Core 3.1 Web API](https://coderjony.com/blogs/adding-swagger-to-aspnet-core-31-web-api/)

[XML Comments Swagger .Net Core](https://medium.com/c-sharp-progarmming/xml-comments-swagger-net-core-a390942d3329)

[How to include XML comments files in Swagger in ASP.NET Core](https://newbedev.com/how-to-include-xml-comments-files-in-swagger-in-asp-net-core)

[A simpler approach to building event driven software using .NET Core and Kafka](https://dev.to/gabrielsadaka/vertical-slice-event-driven-architecture-with-net-core-and-kafka-2ad2) e [.NET Core Kafka Sample](https://github.com/gabrielsadaka/dotnet-kafka-sample)

[Log event datetime with.Net Core Console logger](https://stackoverflow.com/questions/45434653/log-event-datetime-with-net-core-console-logger)