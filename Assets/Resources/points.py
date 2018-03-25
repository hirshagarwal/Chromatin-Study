import csv
import numpy as np
import random
from sys import maxsize

#These are the relevant values to change in the configuration:
min_color_difference = 3
max_color_difference = 6
min_physical_difference = 0.1
max_physical_difference = 0.4
#-------------------------------------------------------------

chromdict = []
normalised = []
chromname = ""
allvalues = np.zeros(0)
maxvalue = 0
minvalue = maxsize
newmax = 0
newmin = maxsize
threshold = 0.2
color_count = 9
largest_chromosome_number = 0
chrid = input("Enter chromosome number: ")
chrnam = input("Enter pro or sen: ")
want_true = (input("Do you want true? Enter 'true' if you want true and 'false' if you don't: "))
if (want_true.lower() == "true"):
    want_true = True
elif (want_true.lower() == "false"):
    want_true = False
chromosome = f"{chrid}_formatted.bed.{chrnam}.txt"

with open(chromosome, 'r') as cfile:
    chromreader = csv.reader(cfile, delimiter='\t')
    for (chrA, chrAStart, chrAEnd, chrB, chrBStart, chrBEnd, val) in chromreader:
        if (int(chrAStart) > largest_chromosome_number):
            largest_chromosome_number = int(chrAStart)
        if (int(chrBStart) > largest_chromosome_number):
            largest_chromosome_number = int(chrBStart)
        val = float(val)
        chromdict += [[val, f"{chrA}-{chrAStart}-{chrAEnd}",f"{chrB}-{chrBStart}-{chrBEnd}"]]
        if (maxvalue < val):
            maxvalue = val
        if (minvalue > val):
            minvalue = val
for row in chromdict:
    colval = (row[0] - minvalue) / (maxvalue - minvalue)
    colval = threshold + ((1 - threshold) * colval)
    if (newmax < colval):
        newmax = colval
    if (newmin > colval):
        newmin = colval
    normalised += [[colval, row[1], row[2]]]

aidx = random.randint(0, len(normalised)-1)
a = normalised[aidx]
a_start = int(a[1].split('-')[1])
a_end = int(a[2].split('-')[1])

while((abs(a_start-a_end)/largest_chromosome_number < min_physical_difference) or (abs(a_start-a_end)/largest_chromosome_number > max_physical_difference)):
    aidx = random.randint(0, len(normalised)-1)
    a = normalised[aidx]
    a_start = int(a[1].split('-')[1])
    a_end = int(a[2].split('-')[1])

a_color = np.floor((a[0] / newmax) * color_count)
random.shuffle(normalised)
b = None

for line in normalised:
    line_color = np.floor((line[0] / newmax) * color_count)
    diff = abs(line_color - a_color)
    if (min_color_difference <= diff <= max_color_difference):
        b_start = int(line[1].split('-')[1])
        b_end = int(line[2].split('-')[1])
        if (abs(b_start-b_end)/largest_chromosome_number >= min_physical_difference):
            if (abs(b_start-b_end)/largest_chromosome_number <= max_physical_difference):
                b = line
                break
try:
    if (a[0]>b[0]):
        if (want_true):
            print(f"{chrid},{chrnam},{a[1]},{a[2]},{b[1]},{b[2]},{a[0]>b[0]}")
        else:
            print(f"{chrid},{chrnam},{b[1]},{b[2]},{a[1]},{a[2]},{b[0]>a[0]}")
    else:
        if (want_true):
            print(f"{chrid},{chrnam},{b[1]},{b[2]},{a[1]},{a[2]},{b[0]>a[0]}")
        else:
            print(f"{chrid},{chrnam},{a[1]},{a[2]},{b[1]},{b[2]},{a[0]>b[0]}")
except:
    print("Configuration not possible for this chromosome with the specifications set. Try another one")