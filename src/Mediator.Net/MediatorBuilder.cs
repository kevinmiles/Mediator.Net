﻿using System;
using System.Linq;
using System.Reflection;
using Mediator.Net.Binding;
using Mediator.Net.Contracts;

namespace Mediator.Net
{
    public class MediatorBuilder
    {
        public MediatorBuilder RegisterHandlersFor(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var commandHandlers = assembly.GetTypes().Where(x => IsAssignableToGenericType(x, typeof(ICommandHandler<>))).ToList();
                commandHandlers.ForEach(x =>
                {
                    MessageHandlerRegistry.Bindings.Add(x.GetInterfaces().First().GenericTypeArguments[0].GetTypeInfo(), x.GetTypeInfo());
                });
            }
            return this;
        }

        private bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}