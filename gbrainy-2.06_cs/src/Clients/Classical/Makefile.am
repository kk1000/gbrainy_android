EXTRAFLAGS =  -unsafe -nowarn:0169 $(CSC_DEFINES) 

WRAPPER = gbrainy
TARGET_CFG = ../../gbrainy.exe.config

GBRAINY_CSDISTFILES =					\
	$(srcdir)/CommandLine.cs 			\
	$(srcdir)/gbrainy.cs 				\
	$(srcdir)/GtkSynchronize.cs 			\
	$(srcdir)/Builder/Builder.cs			\
	$(srcdir)/Builder/BuilderConnectFunc.cs 	\
	$(srcdir)/Builder/GtkBeansSharp.BuilderConnectFuncNative.cs \
	$(srcdir)/Builder/ObjectManager.cs 		\
	$(srcdir)/Dialogs/AboutDialog.cs		\
	$(srcdir)/Dialogs/CustomGameDialog.cs		\
	$(srcdir)/Dialogs/BuilderDialog.cs		\
	$(srcdir)/Dialogs/HigMessageDialog.cs		\
	$(srcdir)/Dialogs/PlayerHistoryDialog.cs	\
	$(srcdir)/Dialogs/PreferencesDialog.cs		\
	$(srcdir)/Dialogs/PdfExportDialog.cs		\
	$(srcdir)/Widgets/GameDrawingArea.cs		\
	$(srcdir)/Widgets/BrowseFile.cs			\
	$(srcdir)/Widgets/Toolbar.cs


ASSEMBLIES = \
	 $(GBRAINY_LIBS)    		\
	 $(MONO_ADDINS_LIBS)    	\
	-r:Mono.Cairo.dll		\
	-r:../../gbrainy.Core.dll	\
	-r:Mono.Posix

RESOURCES =										\
-resource:$(srcdir)/gbrainy.ui \
-resource:$(srcdir)/Dialogs/ui/PreferencesDialog.ui \
-resource:$(srcdir)/Dialogs/ui/PlayerHistoryDialog.ui \
-resource:$(srcdir)/Dialogs/ui/CustomGameDialog.ui \
-resource:$(srcdir)/Dialogs/ui/PdfExportDialog.ui \
-resource:$(top_srcdir)/data/app-graphics/resume-32.png  \
-resource:$(top_srcdir)/data/app-graphics/endgame-32.png  \
-resource:$(top_srcdir)/data/app-graphics/pause-32.png  \
-resource:$(top_srcdir)/data/app-graphics/allgames-32.png  \
-resource:$(top_srcdir)/data/app-graphics/gbrainy.png  \
-resource:$(top_srcdir)/data/app-graphics/logic-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/math-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/memory-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/verbal-games.svg  \
-resource:$(top_srcdir)/data/app-graphics/verbal-games-32.png  \
-resource:$(top_srcdir)/data/app-graphics/gbrainy.svg \
-resource:$(top_srcdir)/data/app-graphics/gtk-info.svg \
-resource:$(top_srcdir)/data/app-graphics/gtk-stop.svg \
-resource:$(top_srcdir)/data/app-graphics/gtk-ok.svg

gbrainydir = $(libdir)/gbrainy
gbrainy_SCRIPTS = ../../gbrainy.exe ../../gbrainy.exe.config
bin_SCRIPTS = gbrainy

GBRAINY_CSFILES = $(GBRAINY_CSDISTFILES)	\
	AssemblyInfo.cs	\
	Defines.cs

../../gbrainy.exe: $(GBRAINY_CSFILES) gbrainy.ui $(srcdir)/Dialogs/ui/PreferencesDialog.ui  \
	$(srcdir)/Dialogs/ui/PlayerHistoryDialog.ui $(srcdir)/Dialogs/ui/CustomGameDialog.ui \
	$(srcdir)/Dialogs/ui/PdfExportDialog.ui
	$(CSC) -target:winexe -out:$@ $(EXTRAFLAGS) $(GBRAINY_CSFILES) $(ASSEMBLIES) $(RESOURCES)

$(TARGET_CFG): gbrainy.exe.config.in
	sed -e "s|\@pkglibdir\@|$(pkglibdir)|" \
	    < $< > $@

all: ../../gbrainy.exe $(TARGET_CFG)

EXTRA_DIST =					\
	gbrainy.ui 				\
	$(srcdir)/gbrainy.exe.config.in 	\
	$(srcdir)/MonoAddinsStrings.cs	 	\
	$(srcdir)/gbrainy.ui 			\
	$(srcdir)/Dialogs/ui/PreferencesDialog.ui \
	$(srcdir)/Dialogs/ui/PlayerHistoryDialog.ui \
	$(srcdir)/Dialogs/ui/CustomGameDialog.ui \
	$(srcdir)/Dialogs/ui/PdfExportDialog.ui \
	$(GBRAINY_CSDISTFILES)


CLEANFILES =					\
	gbrainy.exe.mdb				\
	../../gbrainy.exe			\
	../../gbrainy.exe.config		\
	gbrainy


DISTCLEANFILES = 				\
	Makefile

