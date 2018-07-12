using System;
using System.Diagnostics;
using GraphQL.Dynamic;
using GraphQL.Dynamic.Types.LiteralGraphType;
using GraphQL.StarWars.Types;
using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace GraphQL.StarWars
{
    public class StarWarsQuery : ObjectGraphType<object>
    {
        public StarWarsQuery(StarWarsData data)
        {
            Name = "Query";

            Field<CharacterInterface>("hero", resolve: context => data.GetDroidByIdAsync("3"));
            Field<HumanType>(
                "human",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the human" }
                ),
                resolve: context => data.GetHumanByIdAsync(context.GetArgument<string>("id"))
            );

            Func<ResolveFieldContext, string, object> func = (context, id) => data.GetDroidByIdAsync(id);

            FieldDelegate<DroidType>(
                "droid",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the droid" }
                ),
                resolve: func
            );

            var moniker = "cmdbServer";

            var remotes = new[]
            {
                new RemoteDescriptor
                {
                    Moniker = moniker,
                    Url = "https://cmdb-qs.webservices.gclsintra.net/graphql" //"https://api.giacloud.ch/api/graphql"
                }
            };

            var remoteTypes = RemoteLiteralGraphType.LoadRemotes(remotes).GetAwaiter().GetResult();

            //GiaInfrastructureServiceType
            this.RemoteField(remoteTypes, moniker, "ServerType", "cmdbServer", resolve: ctx =>
            {
                Debug.WriteLine(ctx.Document);
                return JObject.Parse(
                @"{

                        'antimalware': {
                    'antispywareEnabled': true
                        }
                        
                }");
            });
        }
    }
}
