using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using LambdaDynamoDB.Models;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaDynamoDB;

public class Functions
{
    private readonly IAmazonDynamoDB dynamoDB;
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public Functions()
    {
        this.dynamoDB = new AmazonDynamoDBClient();
    }


    /// <summary>
    /// A Lambda function to respond to HTTP Get methods from API Gateway
    /// </summary>
    /// <param name="request"></param>
    /// <returns>The API Gateway response.</returns>
    public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Get Request\n");

        var result = await dynamoDB.ScanAsync(new ScanRequest
        {
            TableName = "SocialDB",
            Limit = 100,
        });

        var users = new List<User>();

        if (result != null && result.Items != null)
        {
            foreach (var item in result.Items)
            {
                item.TryGetValue("PK", out var PK);
                item.TryGetValue("SK", out var SK);
                item.TryGetValue ("attr1", out var attr1);
                item.TryGetValue("attr2", out var attr2);
                item.TryGetValue("attr3", out var attr3);

                users.Add(new User
                {
                    PK = PK.S,
                    SK = SK.S,
                    attr1 = attr1?.S,
                    attr2 = attr2?.S,
                    attr3 = Convert.ToInt32(attr3?.N)
                });
            }
        }

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonConvert.SerializeObject(users),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
    public async Task<APIGatewayProxyResponse> Get2(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Get2 Request\n");
        var prms = request.PathParameters;

        var result = await dynamoDB.GetItemAsync(new GetItemRequest
        {  
            
            TableName = "SocialDB",
            Key = new Dictionary<string, AttributeValue>() { { "PK", new AttributeValue { S = prms["Id"] } }, { "SK", new AttributeValue { S = prms["Id"] } } }
        });

        var user = new User();
        if (result != null && result.Item != null)
        {
            result.Item.TryGetValue("PK", out var PK);
            result.Item.TryGetValue("SK", out var SK);
            result.Item.TryGetValue("attr1", out var attr1);
            result.Item.TryGetValue("attr2", out var attr2);
            result.Item.TryGetValue("attr3", out var attr3);

            user = new User
            {
                PK = PK.S,
                SK = SK.S,
                attr1 = attr1?.S,
                attr2 = attr2?.S,
                attr3 = Convert.ToInt32(attr3?.N)
            };
        }

        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonConvert.SerializeObject(user),
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
    public APIGatewayProxyResponse Post(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Post Request\n");


        var response = new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = "Hello AWS Serverless Post - " + request.Body,
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };

        return response;
    }
}