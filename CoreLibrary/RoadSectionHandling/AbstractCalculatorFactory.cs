using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoreLibrary.RoadSectionHandling
{
    public abstract class AbstractCalculatorFactory
    {

        protected static List<Type> LoadImplementations(AbstractCalculatorAttribute attr, Type assignabeType)
        {

            //Type calculatorType = typeof(ICollisionCalculatorImplementation);

            // Some reflection magic to read all "calculators" from current namespace
            // TODO test this with functional implementation
            Console.WriteLine("Loading Implementations for " + assignabeType.Name);

            List<Type> collisionImplementations = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.GetCustomAttributes<AbstractCalculatorAttribute>()
                        .Any(y => y.Type == attr.Type)).Where(p => assignabeType.IsAssignableFrom(p)).ToList();
            
            Console.WriteLine("Found and loaded " + collisionImplementations.Count + " implementations");

            foreach(Type impl in collisionImplementations)
            {
                Console.WriteLine(impl.Name);
            }
            Console.WriteLine("");
            // Some more magic to create instances of gathered types (we do not need to provide any data to constructors)
            //implKlazzes.ForEach(klazz => curveCollisionImplementations.Add((ICollisionCalculatorImplementation)Activator.CreateInstance(klazz)));
            return collisionImplementations;
        }

        /**
         * Method used to precompile type for faster creation
         * Precompile method with one input parameter
         */
        protected static Func<TArg, object> CreateCreator<TArg>(Type constructorType)
        {
            /*ConstructorInfo constructor = constructorType.GetConstructor(constructorParams);

            ParameterExpression[] expr = new ParameterExpression[constructorParams.Length];

            for (int i= 0; i< constructorParams.Length; i++)
            {
                expr[i] = Expression.Parameter(constructorParams[i], i + "");
            }

            Expression<Func<object[], object>> creatorExpression = Expression.Lambda<Func<object[], object>>(
                            Expression.New(constructor, expr), expr);
            return creatorExpression.Compile();*/

            var constructor = constructorType.GetConstructor(new Type[] { typeof(TArg) });
            var parameter = Expression.Parameter(typeof(TArg), "p");
            var creatorExpression = Expression.Lambda<Func<TArg, object>>(
                Expression.New(constructor, new Expression[] { parameter }), parameter);
            return creatorExpression.Compile();

        }

        /**
         * Method used to precompile type for faster creation
         */
        protected static Func<object> CreateCreator(Type constructorType)
        {

            var constructor = constructorType.GetConstructor(new Type[] { });
            var creatorExpression = Expression.Lambda<Func<object>>(
                Expression.New(constructor));
            return creatorExpression.Compile();

        }

        abstract public List<Type> GetLoadedTypes();
    }
}
