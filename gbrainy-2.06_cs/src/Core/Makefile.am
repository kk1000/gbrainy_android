CSFLAGS = -unsafe -target:library $(CSC_DEFINES)

#Active when designing games
#CSFLAGS += -D:DESIGN_MODE

TARGET = ../gbrainy.Core.dll
TARGET_CFG = ../gbrainy.Core.dll.config

CSDISTFILES =  \
		$(srcdir)/Main/ArrayListIndicesRandom.cs	\
		$(srcdir)/Main/CairoContextEx.cs	\
		$(srcdir)/Main/ColorPalette.cs		\
		$(srcdir)/Main/IMouseEvent.cs		\
		$(srcdir)/Main/Game.cs			\
		$(srcdir)/Main/GameAnswer.cs		\
		$(srcdir)/Main/GameAnswerCheckAttributes.cs \
		$(srcdir)/Main/GameAnswerEventArgs.cs	\
		$(srcdir)/Main/GameDifficulty.cs 	\
		$(srcdir)/Main/GameManager.cs		\
		$(srcdir)/Main/GameSession.cs		\
		$(srcdir)/Main/GameSessionHistory.cs	\
		$(srcdir)/Main/GameSessionHistoryExtended.cs \
		$(srcdir)/Main/GameTypes.cs		\
		$(srcdir)/Main/GameTips.cs		\
		$(srcdir)/Main/Xml/DrawingObject.cs	\
		$(srcdir)/Main/Xml/ImageDrawingObject.cs \
		$(srcdir)/Main/Xml/LocalizableString.cs	\
		$(srcdir)/Main/Xml/GameXml.cs		\
		$(srcdir)/Main/Xml/GameXmlFactory.cs	\
		$(srcdir)/Main/Xml/GameXmlDefinition.cs	\
		$(srcdir)/Main/Xml/GameXmlDefinitionVariant.cs	\
		$(srcdir)/Main/Xml/OptionDrawingObject.cs \
		$(srcdir)/Main/Xml/TextDrawingObject.cs	\
		$(srcdir)/Main/Memory.cs		\
		$(srcdir)/Main/PdfExporter.cs		\
		$(srcdir)/Main/PlayerHistory.cs		\
		$(srcdir)/Main/PlayerPersonalRecord.cs	\
		$(srcdir)/Main/Preferences.cs		\
		$(srcdir)/Main/PreferencesStorage.cs	\
		$(srcdir)/Main/Score.cs			\
		$(srcdir)/Main/Theme.cs			\
		$(srcdir)/Main/ThemeManager.cs		\
		$(srcdir)/Main/UpdateUIStateEventArgs.cs \
		$(srcdir)/Main/Verbal/Analogies.cs 	\
		$(srcdir)/Main/Verbal/AnalogiesFactory.cs	\
		$(srcdir)/Main/Verbal/AnalogiesMultipleOptions.cs	\
		$(srcdir)/Main/Verbal/AnalogiesPairOfWordsCompare.cs	\
		$(srcdir)/Main/Verbal/AnalogiesPairOfWordsOptions.cs	\
		$(srcdir)/Main/Verbal/AnalogiesQuestionAnswer.cs		\
		$(srcdir)/Main/Verbal/Analogy.cs		\
		$(srcdir)/Views/CountDownView.cs		\
		$(srcdir)/Views/FinishView.cs		\
		$(srcdir)/Main/IDrawable.cs		\
		$(srcdir)/Main/IDrawRequest.cs		\
		$(srcdir)/Toolkit/Container.cs		\
		$(srcdir)/Toolkit/DrawableArea.cs		\
		$(srcdir)/Toolkit/DrawEventArgs.cs	\
		$(srcdir)/Toolkit/HorizontalContainer.cs	\
		$(srcdir)/Toolkit/SeletectedEventArgs.cs	\
		$(srcdir)/Toolkit/Widget.cs		\
		$(srcdir)/Views/PlayerHistoryView.cs	\
		$(srcdir)/Views/ViewsControler.cs		\
		$(srcdir)/Views/WelcomeView.cs		\
		$(srcdir)/Platform/Unix.cs		\
		$(srcdir)/Libraries/CairoContext.cs	\
		$(srcdir)/Libraries/GetText.cs		\
		$(srcdir)/Libraries/SVGImage.cs		\
		$(srcdir)/Services/IService.cs		\
		$(srcdir)/Services/ServiceLocator.cs	\
		$(srcdir)/Services/ITranslations.cs	\
		$(srcdir)/Libraries/TranslationsCatalog.cs \
		$(srcdir)/Services/IConfiguration.cs	\
		$(srcdir)/Services/MemoryConfiguration.cs \
		$(srcdir)/Services/DefaultServices.cs	\
		$(srcdir)/Services/ICSharpCompiler.cs	\
		$(srcdir)/Libraries/CSharpCompilerStatic.cs	\
		$(srcdir)/Libraries/CSharpCompilerStaticDomainProxy.cs	\
		$(srcdir)/Libraries/CSharpCompiler.cs

CSFILES = $(CSDISTFILES)	\
	Main/Defines.cs	\
	AssemblyInfo.cs

RES = \
$(top_srcdir)/data/app-graphics/logic-games.svg		\
$(top_srcdir)/data/app-graphics/math-games.svg		\
$(top_srcdir)/data/app-graphics/memory-games.svg	\
$(top_srcdir)/data/app-graphics/verbal-games.svg	\
$(srcdir)/gbrainy.addin.xml


REFS =  \
	-r:System \
	-r:Mono.Cairo.dll		\
	-r:Mono.Posix			\
	-r:Mono.CSharp.dll		\
	 $(GBRAINY_LIBS)    		\
	 $(MONO_ADDINS_LIBS)

RES_CSFLAGS = $(foreach res, $(RES), -resource:$(res))

$(TARGET): $(CSFILES) $(RES)
	echo $(RES_FILES)
	$(CSC) -out:$@ $(CSFLAGS) $(REFS) $(RES_CSFLAGS) $(CSFILES)

$(TARGET_CFG): $(srcdir)/gbrainy.Core.dll.config.in
	sed -e "s|\@pkglibdir\@|$(pkglibdir)|" \
	    < $< > $@

all: $(TARGET) $(TARGET_CFG)

install-data-local: $(TARGET) $(TARGET_CFG)
	$(mkinstalldirs) $(DESTDIR)$(pkglibdir)
	$(INSTALL_DATA) $(TARGET) $(DESTDIR)$(pkglibdir)
	$(INSTALL_DATA) $(TARGET_CFG) $(DESTDIR)$(pkglibdir)

uninstall-local:
	cd $(DESTDIR)$(pkglibdir) && rm -f gbrainy.Core.dll
	cd $(DESTDIR)$(pkglibdir) && rm -f gbrainy.Core.dll.config

EXTRA_DIST = 			\
	$(CSDISTFILES) $(RES) gbrainy.Core.dll.config.in

CLEANFILES =				\
	$(TARGET)			\
	$(TARGET).mdb			\
	$(TARGET_CFG)
