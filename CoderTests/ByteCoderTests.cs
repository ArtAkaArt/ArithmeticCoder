﻿using NUnit.Framework;
using FluentAssertions;
using ArithmeticCoder;

namespace ArithmeticCoderTests
{
    [TestFixture]
    public class ByteCoderTests
    {
        private int[] field;
        private int[] sameFiguresField;
        private int[] equalCoeffs;

        [SetUp]
        public void Setup()
        {
            field = new int[]
            {
                0, 1, 2, 3, 4, 0, 1, 2, 3, 4,
                0, 1, 2, 3, 4, 0, 1, 2, 3, 4,
                0, 1, 2, 3, 4, 0, 1, 2, 3, 4,
                4, 4,
            };
            equalCoeffs = new[] { 2, 2, 2, 2, 2 };

            sameFiguresField = new int[]
            {
                4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                4, 4,
            };
        }

        [Test]
        public void EqualDistributionInput()
        {
            var codedField = field.ByteEncode(equalCoeffs);
            var decodedField = codedField.ByteDecode(equalCoeffs);

            decodedField.Should().BeEquivalentTo(field);
        }

        [Test]
        public void HeavelyShiftedToLastFigure()
        {
            var coefs = new int[] { 1, 1, 1, 1, 50 };

            var codedField = sameFiguresField.ByteEncode(coefs);
            var decodedField = codedField.ByteDecode(coefs);

            decodedField.Should().BeEquivalentTo(sameFiguresField);
        }

        [Test]
        public void HeavelyShiftedToFirstFigure()
        {
            var coefs = new int[] { 50, 1, 1, 1, 1 };

            var codedField = sameFiguresField.Select(x => x - 4).ToArray().ByteEncode(coefs);
            var decodedField = codedField.ByteDecode(coefs);

            decodedField.Should().BeEquivalentTo(sameFiguresField.Select(x => x - 4).ToArray());
        }

        [Test]
        public void CloseToRealDistributionStartingField()
        {
            var coefs = new int[] { 4, 3, 3, 1, 1 };

            var legitField = new int[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
                2, 2
            };

            var codedField = legitField.ByteEncode(coefs);
            var decodedField = codedField.ByteDecode(coefs);

            decodedField.Should().BeEquivalentTo(legitField);
        }

        [Test]
        public void CloseToRealDistributionAfterSomeMoves()
        {
            var coefs = new int[] { 5, 3, 3, 1, 1 };

            var legitField = new int[]
            {
                0, 1, 0, 2, 0, 3, 0, 0, 1, 1,
                0, 0, 0, 1, 1, 1, 1, 1, 1, 1,
                0, 0, 0, 2, 2, 2, 2, 2, 2, 2,
                4, 3
            };

            var codedField = legitField.ByteEncode(coefs);
            var decodedField = codedField.ByteDecode(coefs);

            decodedField.Should().BeEquivalentTo(legitField);
        }

        [Test]
        public void HeavelyShiftedToLastFigureWithHighFirstCoeff()
        {
            var coefs = new int[] { 20, 1, 1, 1, 1 };

            var codedField = sameFiguresField.ByteEncode(coefs);
            var decodedField = codedField.ByteDecode(coefs);

            decodedField.Should().BeEquivalentTo(sameFiguresField);
        }
    }
}