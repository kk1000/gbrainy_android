AC_INIT(gbrainy, 2.06)
AC_CONFIG_SRCDIR(src/Core/)

# Version for assemblies: Major.Minor.Build.Revision
ASSEMBLY_VERSION="2.0.6.0"
AC_SUBST(ASSEMBLY_VERSION)

AM_CONFIG_HEADER(config.h)

AM_INIT_AUTOMAKE([tar-ustar])

AM_MAINTAINER_MODE

IT_PROG_INTLTOOL([0.35.0])

AC_PROG_CC
AC_ISC_POSIX
AC_HEADER_STDC
AM_PROG_LIBTOOL

GNOME_COMPILE_WARNINGS
AM_PATH_GLIB_2_0

dnl --- Check for mono and dmcs or gmcs

AC_PATH_PROG(MONO, mono)

AC_PATH_PROG(DMCS, dmcs, "")
AC_PATH_PROG(CSC, gmcs, $DMCS)

CSC_DEFINES=""
CS="C#"
if test ! -x "$CSC"; then
	AC_MSG_ERROR([No $CS compiler found])
fi

dnl -- Initialize docs

GNOME_DOC_INIT

dnl -- Check for mono pc file

AC_MSG_CHECKING([for mono.pc])
if test -z `$PKG_CONFIG --variable=prefix mono`; then
  AC_MSG_ERROR([missing the mono.pc file, usually found in the mono-devel package])
else
  AC_MSG_RESULT([found])
fi

if ! pkg-config --atleast-version=2.11 mono; then
   CSC_DEFINES="$CSC_DEFINES -d:CSHARP_STATIC"
   csharp_static="Yes"
else
   csharp_static="No"
fi


dnl -- check for various mono assemblies that we need.

needed_assemblies="Mono.Posix Mono.Cairo Mono.CSharp"
for i in $needed_assemblies; do
  AC_MSG_CHECKING([for $i.dll])
  if test ! \( -e `$PKG_CONFIG --variable=prefix mono`/lib/mono/2.0/$i.dll -o -e `$PKG_CONFIG --variable=prefix mono`/lib64/mono/2.0/$i.dll \); then
	if test ! \( -e `$PKG_CONFIG --variable=prefix mono`/lib/mono/4.0/$i.dll -o -e `$PKG_CONFIG --variable=prefix mono`/lib64/mono/4.0/$i.dll \); then
		AC_MSG_ERROR([missing required mono 2.0/4.0 DLL: $i.dll])
	else
	AC_MSG_RESULT([found])
	fi
  else
	AC_MSG_RESULT([found])
  fi
done

dnl --- Required libraries

GTKSHARP_REQUIRED=2.10
MONO_REQUIRED=1.1.7
MONO_ADDINS=0.3

PKG_CHECK_MODULES(GBRAINY_CORE, mono >= $MONO_REQUIRED)


PKG_CHECK_MODULES(GBRAINY, gtk-sharp-2.0 >= $GTKSHARP_REQUIRED)
AC_SUBST(GBRAINY_LIBS)

AC_ARG_ENABLE(addins, AC_HELP_STRING([--disable-addins], [Disable mono-addins support]), , enable_addins="yes")

enable_addins_sharp="${enable_addins}"

PKG_CHECK_MODULES(MONO_ADDINS,
	mono-addins >= $MONO_ADDINS
	mono-addins-setup >= $MONO_ADDINS
	mono-addins-gui >= $MONO_ADDINS,
	enable_addins_sharp="$enable_addins_sharp", enable_addins_sharp=no)

if test "x$enable_addins" = "xyes" -a "x$enable_addins_sharp" = "xno"; then
	AC_MSG_ERROR([mono-addins was not found or is not up to date. Please install mono-addins of at least version $MONO_ADDINS, or disable mono-addins support by --disable-addins])
fi

if test "x$enable_addins_sharp" = "xyes"; then
	AC_SUBST(MONO_ADDINS_LIBS)
	CSC_DEFINES="$CSC_DEFINES -d:MONO_ADDINS"
fi

AM_CONDITIONAL(ENABLE_ADDINS, test "x$enable_addins_sharp" = "xyes")

PKG_CHECK_MODULES(GBRAINY_CHECK, librsvg-2.0)

dnl -- Intl

GETTEXT_PACKAGE=gbrainy
AC_SUBST(GETTEXT_PACKAGE)
AC_DEFINE_UNQUOTED(GETTEXT_PACKAGE,"$GETTEXT_PACKAGE", [Gettext package])
AM_GLIB_GNU_GETTEXT


dnl -- NUnit required for (optional) unit tests
NUNIT_REQUIRED=2.4.7

AC_ARG_ENABLE(tests, AC_HELP_STRING([--enable-tests], [Enable NUnit tests]),
	enable_tests=$enableval, enable_tests="no")

if test "x$enable_tests" = "xno"; then
	do_tests=no
	AM_CONDITIONAL(ENABLE_TESTS, false)
else
	PKG_CHECK_MODULES(NUNIT, nunit >= $NUNIT_REQUIRED,
		do_tests="yes", do_tests="no")

	AC_SUBST(NUNIT_LIBS)
	AC_PATH_PROG(NUNIT, nunit-console)

	if test ! -x "$NUNIT"; then
		AC_MSG_ERROR([Could not find nunit console binary])
	fi

	AM_CONDITIONAL(ENABLE_TESTS, test "x$do_tests" = "xyes")

	if test "x$do_tests" = "xno"; then
		PKG_CHECK_MODULES(NUNIT, mono-nunit >= 2.4,
			do_tests="yes", do_tests="no")

		AC_SUBST(NUNIT_LIBS)
		AM_CONDITIONAL(ENABLE_TESTS, test "x$do_tests" = "xyes")

		if test "x$do_tests" = "xno"; then
			AC_MSG_ERROR([Could not find nunit libraries])
		fi
	fi
fi

dnl --- Debugging support

AC_ARG_ENABLE(debug,
	AC_HELP_STRING([--enable-debug],
		[Use 'Debug' Configuration [default=no]]),
		enable_debug=$enableval, enable_debug=no)
AM_CONDITIONAL(ENABLE_DEBUG, test "x$enable_debug" != "xno")

if test "x$enable_debug" != "xno" ; then
	CSC_DEFINES="$CSC_DEFINES -debug -d:DEBUG"
fi

dnl --- Assembly information

BUILD_TIME=`date +"%F %T %Z"`
AC_SUBST(BUILD_TIME)

COPYRIGHT="(c) 2007-2011 Jordi Mas i Hernandez"
AC_SUBST(COPYRIGHT)

WEB_SITE="http://www.gbrainy.com"
AC_SUBST(WEB_SITE)

dnl --- Prologue

AC_SUBST(CFLAGS)
AC_SUBST(CPPFLAGS)
AC_SUBST(LDFLAGS)

AC_SUBST(CSC)
AC_SUBST(CSC_DEFINES)

expanded_libdir=`( case $prefix in NONE) prefix=$ac_default_prefix ;; *) ;; esac
		   case $exec_prefix in NONE) exec_prefix=$prefix ;; *) ;; esac
		   eval echo $libdir )`
AC_SUBST(expanded_libdir)

AC_CONFIG_FILES([src/Clients/Classical/gbrainy],[chmod +x src/Clients/Classical/gbrainy])

GNOME_ICON_THEME_PREFIX=`$PKG_CONFIG --variable=prefix gnome-icon-theme`
AC_SUBST(GNOME_ICON_THEME_PREFIX)

AC_OUTPUT([
src/Core/Main/Defines.cs
src/Clients/Classical/Defines.cs
src/Clients/Classical/AssemblyInfo.cs
src/Core/AssemblyInfo.cs
src/Games/AssemblyInfo.cs
Makefile
po/Makefile.in
src/Makefile
tools/Makefile
src/Core/Makefile
src/Games/Makefile
src/Clients/Classical/Makefile
tests/Makefile
data/Makefile
data/gbrainy.pc
help/Makefile
tools/Defines.cs
])

echo "Mono-addins:		${enable_addins_sharp}"
echo "NUnit:			${enable_tests}"
echo "Debug enabled:		${enable_debug}"
echo "Mono.CSharp static:	${csharp_static}"
