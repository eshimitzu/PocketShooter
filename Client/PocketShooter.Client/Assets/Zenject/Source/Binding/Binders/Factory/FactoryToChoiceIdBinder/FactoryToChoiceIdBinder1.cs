﻿using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class FactoryToChoiceIdBinder<TParam1, TContract> : FactoryArgumentsToChoiceBinder<TParam1, TContract>
    {
        public FactoryToChoiceIdBinder(
            DiContainer bindContainer, BindInfo bindInfo, FactoryBindInfo factoryBindInfo)
            : base(bindContainer, bindInfo, factoryBindInfo)
        {
        }

        public FactoryArgumentsToChoiceBinder<TParam1, TContract> WithId(object identifier)
        {
            BindInfo.Identifier = identifier;
            return this;
        }
    }
}
