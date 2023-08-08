Start elastic search:
```
docker compose up -d
```

Create index and populate english vocabulary:
```
dotnet run --project DataGenerator\ElasticTest.DataGenerator.csproj
```

Run web API:
```
dotnet run --project ElasticTest\ElasticTest.csproj
```