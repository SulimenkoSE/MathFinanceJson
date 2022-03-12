using Xunit;
using MathJson;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using NSubstitute;

namespace TestProjectMathFinanceJson
{
    public class LineaeRegressionTest
    {

        //Создаем набор данных для передачи в тесты.После запятой предполагаемый результат
        
        public static IEnumerable<object[]> newDataFaild
        {
            get
            {
                yield return new object[]{ 
                    new double[] { 0,1,2,3,4},
                    new double[] { 0,1,2,3,4,5},
                    1, 0, 1 //ожидаемый результат result
                };
            }
        }

        public static IEnumerable<object[]> newData
        {
            get
            {
                yield return new object[]{
                    new double[] { 0,1,2,3,4},
                    new double[] { 0,1,2,3,4},
                    1, 0, 1 //ожидаемый результат result
                };

                yield return new object[]{
                    new double[] 
                    {
                        1990, 1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004,
                        2005, 2006, 2007, 2008, 2009
                    },
                    new double[] 
                    {
                        8669269, 8595500, 8484900, 8459800, 8427400, 8384700, 8340900, 8283200, 8230400, 8190900,
                        8149468, 7932984, 7845841, 7801273, 7761049, 7720000, 7679290, 7640238, 7606551, 7563710
                    },
                    0.97991267658047987, 129751835.40300752, -60846.945112781956 //ожидаемый результат result
                };
            }
        }

        [Trait("LearRegression", "Throws")]
        [MemberData(nameof(newDataFaild))]
        [Theory]
        public void LearRegression_Throw(
            double[] xVals, 
            double[] yVals,
            double resultrSquared,
            double resultintercept,
            double resultslope)
        {
            double rSquared = default;
            double intercept = default;
            double slope = default;
            var sut = new LinearRegression();
            Action diffsizes = () => sut.LinearRegressions(xVals, yVals, out rSquared, out intercept, out slope);
            diffsizes.Should().Throw<Exception>();

        }
        [Trait("LearRegression", "Throws")]
        [MemberData(nameof(newData))]
        [Theory]
        public void LearRegression_Correct(
            double[] xVals,
            double[] yVals,
            double resultrSquared,
            double resultintercept,
            double resultslope)
        {
            double rSquared = default;
            double intercept = default;
            double slope = default;
            var sut = new LinearRegression();
            sut.LinearRegressions(xVals, yVals, out rSquared, out intercept, out slope);
            rSquared.Should().Be(resultrSquared);
            intercept.Should().Be(resultintercept);
            slope.Should().Be(resultslope);

        }
    }
}
