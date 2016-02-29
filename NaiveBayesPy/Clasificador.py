#!/usr/bin/python

class Clasificador():
    """description of class"""
    tipo = ""
    contador = {}
    totalPalabras = 0
    totalTuits = 0

    def __init__(self, tipo):
        self.tipo
    

    def laContiene(self, palabra):
        charInvalidos = ['-', '*', '+', '.', ',', '!', ')', ':', '(', '"', ';', '#', '@']
        for caracter in charInvalidos:
            if caracter in palabra:
                palabra = str.replace(palabra, '')
        return palabra

    def QuitarRepeticion(self, palabra):
        nuevaPalabra = ""
        for i in xrange(0,len(palabra)):
            if palabra[i] == palabra[i+1] and \
                (palabra[i+1] != 'c' or palabra[i+1] != 'r' or palabra[i+1] != 'l'):
                i += 1
            nuevaPalabra += palabra[i]
        return nuevaPalabra

    def corregirPalabra(self, palabra):
        palabra = QuitarRepeticion(palabra)

    def ContarPalabras(self, tuit,positivo, tipo):
        self.totalTuits += 1
        palabras = str.split(tuit)
        for palabra in xrange(0,len(palabras)):
            if (positivo == True and tipo == True) \
                or (positivo == False and tipo == False):
                self.totalPalabras += 1
                word = laContiene(palabra)
                if len(word) == 0:
                    continue
                

