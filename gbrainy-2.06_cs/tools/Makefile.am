EXTRAFLAGS = $(CSC_DEFINES)

GAMESXMLSTRINGS_CSFILES = \
	$(srcdir)/GameXmlToGetString.cs

TRANSLATIONSCHECKER_CSFILES = \
	$(srcdir)/TranslationsChecker.cs \
	$(srcdir)/GetTextParser/CatalogParser.cs \
	$(srcdir)/GetTextParser/StringEscaping.cs

GAMESSTATISTICS_CSFILES = \
	$(srcdir)/Defines.cs	\
	$(srcdir)/GamesStatistics.cs

GAMEXMLTOASSEMBLY_CSFILES = \
	$(srcdir)/GameXmlToAssembly.cs


ASSEMBLIES = \
	-r:../src/gbrainy.Core.dll	\
	-r:Mono.Posix

MONO_PATH = $(top_builddir)/src

GameXmlToGetString.exe: $(GAMESXMLSTRINGS_CSFILES) ../data/games.xml $(srcdir)/GameXmlGetStringTemplate.cs
		$(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(GAMESXMLSTRINGS_CSFILES) $(ASSEMBLIES)
		export MONO_PATH=$(MONO_PATH) && $(MONO) $@ $(srcdir)

TranslationsChecker.exe: $(TRANSLATIONSCHECKER_CSFILES)
		$(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(TRANSLATIONSCHECKER_CSFILES) $(ASSEMBLIES)

GamesStatistics.exe: $(GAMESSTATISTICS_CSFILES)
		$(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(GAMESSTATISTICS_CSFILES) $(ASSEMBLIES)

GameXmlToAssembly.exe: $(GAMEXMLTOASSEMBLY_CSFILES) ../data/games.xml GameTemplate.cs
		   $(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(GAMEXMLTOASSEMBLY_CSFILES) $(ASSEMBLIES)
		   export MONO_PATH=../src && $(MONO) $@


all: GameXmlToGetString.exe

run-stats: 	GamesStatistics.exe
		export MONO_PATH=$(MONO_PATH) && $(MONO) GamesStatistics.exe $(srcdir)

EXTRA_DIST = $(GAMESXMLSTRINGS_CSFILES) $(srcdir)/GameXmlGetStringTemplate.cs  $(srcdir)/GameXmlGetString.cs

CLEANFILES = GameXmlToGetString.exe \
		GameXmlGetString.cs \
	     	GamesStatistics.exe \
		GameXmlToAssembly.exe

DISTCLEANFILES = 				\
	Makefile

