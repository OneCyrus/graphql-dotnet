using System;
using GraphQL.StarWars.Types;
using GraphQL.Types;
using GraphQL.Dynamic;
using Newtonsoft.Json.Linq;
using GraphQL.Dynamic.Types.LiteralGraphType;

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

            var moniker = "customerTemp";

            var remotes = new[]
            {
                new RemoteDescriptor
                {
                    Moniker = moniker,
                    Url = "https://api.giacloud.ch/api/graphql"
                }
            };

            var remoteTypes = RemoteLiteralGraphType.LoadRemotes(remotes).GetAwaiter().GetResult();

            this.RemoteField(remoteTypes, moniker, "GiaCustomerType", "customer", resolve: ctx =>
            {
                // This is where we'd hypothetically return a JObject result from the remote server

                return JObject.FromObject(new
                {
                    user = new
                    {
                        login = "foo@bar.com",
                        id = 12,
                        company = "some company",
                        repos = new[]
                        {
                                new
                                {
                                    id = 22,
                                    name = "qux"
                                },
                                new
                                {
                                    id = 52,
                                    name = "baz"
                                }
                            }
                    }
                });
            });
        }
    }
}
