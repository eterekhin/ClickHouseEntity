using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DbContext
{
    public class AnonymousClassNameValueListToObject : INameValueListToObject
    {
        // todo кэшировать лямбду
        public T Build<T>(IEnumerable<NameValue> cells)
        {
            var props = typeof(T).GetProperties();
            // строим констуктор
            var cellProps = props.Join(cells, x => x.Name, x => x.Name,
                (x, y) => new {type = x.PropertyType, value = y.Value});
            var constantExpressions =
                cellProps.Select(x => Expression.Convert(Expression.Constant(x.value), x.type));
            var ctor = typeof(T).GetConstructors().Single();
            var expressionNew = Expression.New(ctor, constantExpressions);
            var ctorLambda = Expression.Lambda<Func<T>>(expressionNew);
            return ctorLambda.Compile().Invoke();
        }
    }
}