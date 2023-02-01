obj = src/Binasc.o src/MidiEvent.o src/MidiEventList.o src/MidiFile.o src/MidiMessage.o src/Options.o
CXXFLAG = -O3 -std=c++11
LDFLAG = --shared --static

all: midiParser.dll
src/%.o: src/%.cpp
	g++ $< -o $@ -c -I.\include $(CXXFLAG)
tools/%.o: tools/%.cpp
	g++ $< -o $@ -c -I.\include $(CXXFLAG)
%.dll: tools/%.o $(obj)
	g++ $< $(obj) -o $@ $(LDFLAG)
