using System;
using GraphQL.StarWars.Types;
using GraphQL.Types;
using GraphQL.Dynamic;
using Newtonsoft.Json.Linq;
using GraphQL.Dynamic.Types.LiteralGraphType;
using GraphQL.Validation;
using System.Collections.Generic;
using GraphQL.Language.AST;
using System.Diagnostics;

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

            //GiaInfrastructureServiceType
            this.RemoteField(remoteTypes, moniker, "GiaCustomerType", "customer", resolve: ctx =>
            {
                Debug.WriteLine(ctx.Document);
                return JObject.Parse(
                "{ }");
            });
        }
    }
}
