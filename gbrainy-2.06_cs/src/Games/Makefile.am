CSFLAGS = -target:library 

TARGET = ../gbrainy.Games.dll

CSDISTFILES =  \
		$(srcdir)/GameList.cs					\
		$(srcdir)/Logic/Puzzle3DCube.cs				\
		$(srcdir)/Logic/PuzzleBalance.cs				\
		$(srcdir)/Logic/PuzzleBuildTriangle.cs			\
		$(srcdir)/Logic/PuzzleCirclesSquare.cs			\
		$(srcdir)/Logic/PuzzleClocks.cs				\
		$(srcdir)/Logic/PuzzleCountCircles.cs			\
		$(srcdir)/Logic/PuzzleCounting.cs				\
		$(srcdir)/Logic/PuzzleCountSeries.cs			\
		$(srcdir)/Logic/PuzzleCoverPercentage.cs			\
		$(srcdir)/Logic/PuzzleCube.cs				\
		$(srcdir)/Logic/PuzzleDice.cs				\
		$(srcdir)/Logic/PuzzleDivideCircle.cs			\
		$(srcdir)/Logic/PuzzleEquation.cs				\
		$(srcdir)/Logic/PuzzleExtraCircle.cs			\
		$(srcdir)/Logic/PuzzleFigureLetter.cs			\
		$(srcdir)/Logic/PuzzleFigurePattern.cs			\
		$(srcdir)/Logic/PuzzleFigures.cs				\
		$(srcdir)/Logic/PuzzleFourSided.cs			\
		$(srcdir)/Logic/PuzzleHandshakes.cs			\
		$(srcdir)/Logic/PuzzleLargerShape.cs			\
		$(srcdir)/Logic/PuzzleLines.cs				\
		$(srcdir)/Logic/PuzzleGridCircles.cs			\
		$(srcdir)/Logic/PuzzleGridNumbers.cs			\
		$(srcdir)/Logic/PuzzleMissingPiece.cs			\
		$(srcdir)/Logic/PuzzleMissingSlice.cs			\
		$(srcdir)/Logic/PuzzleMostInCommon.cs			\
		$(srcdir)/Logic/PuzzleMoveFigure.cs			\
		$(srcdir)/Logic/PuzzleNextFigure.cs			\
		$(srcdir)/Logic/PuzzleNumericRelation.cs			\
		$(srcdir)/Logic/PuzzleNumericSequence.cs			\
		$(srcdir)/Logic/PuzzleOstracism.cs			\
		$(srcdir)/Logic/PuzzlePencil.cs				\
		$(srcdir)/Logic/PuzzlePeopleTable.cs			\
		$(srcdir)/Logic/PuzzlePercentage.cs			\
		$(srcdir)/Logic/PuzzleQuadrilaterals.cs			\
		$(srcdir)/Logic/PuzzleGridDots.cs			\
		$(srcdir)/Logic/PuzzleSquaresAndLetters.cs		\
		$(srcdir)/Logic/PuzzleSquares.cs				\
		$(srcdir)/Logic/PuzzleSquareSheets.cs			\
		$(srcdir)/Logic/PuzzleTetris.cs				\
		$(srcdir)/Logic/PuzzleTimeNow.cs				\
		$(srcdir)/Logic/PuzzleTrains.cs				\
		$(srcdir)/Logic/PuzzleTriangles.cs			\
		$(srcdir)/Logic/PuzzleTrianglesWithNumbers.cs		\
		$(srcdir)/Logic/PuzzlePredicateLogic.cs			\
		$(srcdir)/Logic/PuzzleRelatedNumbers.cs			\
		$(srcdir)/Memory/MemoryColouredFigures.cs		\
		$(srcdir)/Memory/MemoryColouredText.cs			\
		$(srcdir)/Memory/MemoryCountDots.cs			\
		$(srcdir)/Memory/MemoryFacts.cs				\
		$(srcdir)/Memory/MemoryFigures.cs				\
		$(srcdir)/Memory/MemoryFiguresAndText.cs			\
		$(srcdir)/Memory/MemoryFiguresNumbers.cs			\
		$(srcdir)/Memory/MemoryIndications.cs			\
		$(srcdir)/Memory/MemoryNumbers.cs				\
		$(srcdir)/Memory/MemoryWords.cs				\
		$(srcdir)/Calculation/CalculationArithmetical.cs		\
		$(srcdir)/Calculation/CalculationAverage.cs		\
		$(srcdir)/Calculation/CalculationCloserFraction.cs 	\
		$(srcdir)/Calculation/CalculationFractions.cs		\
		$(srcdir)/Calculation/CalculationGreatestDivisor.cs 	\
		$(srcdir)/Calculation/CalculationOperator.cs		\
		$(srcdir)/Calculation/CalculationPrimes.cs		\
		$(srcdir)/Calculation/CalculationProportions.cs		\
		$(srcdir)/Calculation/CalculationRatio.cs		\
		$(srcdir)/Calculation/CalculationTwoNumbers.cs

CSFILES = $(CSDISTFILES)	\
	AssemblyInfo.cs

RES = \
$(top_srcdir)/data/game-graphics/present.svg  	\
$(top_srcdir)/data/game-graphics/handshake.svg	\
$(top_srcdir)/data/game-graphics/dice.svg	\
$(top_srcdir)/data/game-graphics/train_right.svg \
$(top_srcdir)/data/game-graphics/train_left.svg \
$(top_srcdir)/data/game-graphics/tv_set.svg \
$(top_srcdir)/data/game-graphics/bucket.svg  \
$(top_srcdir)/data/game-graphics/shop.svg


REFS =  \
	-r:System \
	-r:Mono.Cairo.dll		\
	-r:Mono.Posix			\
	-r:.././gbrainy.Core.dll

RES_CSFLAGS = $(foreach res, $(RES), -resource:$(res))

$(TARGET): $(CSFILES) $(RES)
	echo $(RES_FILES)
	$(CSC) -out:$@ $(CSFLAGS) $(REFS) $(RES_CSFLAGS) $(CSFILES)

all: $(TARGET)

install-data-local: $(TARGET)
	$(mkinstalldirs) $(DESTDIR)$(pkglibdir)
	$(INSTALL_DATA) $(TARGET) $(DESTDIR)$(pkglibdir)

uninstall-local:
	cd $(DESTDIR)$(pkglibdir) && rm -f gbrainy.Games.dll

EXTRA_DIST = 			\
	$(CSDISTFILES) $(RES)

CLEANFILES =			\
	$(TARGET)		\
	$(TARGET).mdb

