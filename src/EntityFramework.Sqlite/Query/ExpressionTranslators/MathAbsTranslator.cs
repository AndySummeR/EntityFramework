﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Entity.Query.ExpressionTranslators;

namespace Microsoft.Data.Entity.Sqlite.Query.ExpressionTranslators
{
    public class MathAbsTranslator : MultipleOverloadStaticMethodCallTranslator
    {
        public MathAbsTranslator()
            : base(typeof(Math), "Abs", "abs")
        {
        }
    }
}