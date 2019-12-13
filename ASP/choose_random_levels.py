import json
import os
from random import randint
import sys

curr_folder = os.path.dirname(os.path.abspath(__file__))
output_file = os.path.join(curr_folder, 'output.json')

output_jsons = []

sys.stdout.write("\rLoading bulk JSON")
sys.stdout.flush()

with open(output_file, "r") as read_file:
	data = json.load( read_file)
	levels = data['Call'][0]['Witnesses']
	sys.stdout.write("\rJSON Loaded, segmenting JSONs\n")
	sys.stdout.flush()

	num_lvl_segs = 10
	for i in range(num_lvl_segs):
		sys.stdout.write("\r{} of {} formatted".format(i+1, num_lvl_segs))
		sys.stdout.flush()    	
		curr_dict = {}
		curr_dict['Levels'] = []
		for level in range(1000):
			level_index = randint(0, len(levels))
			curr_dict['Levels'].append(levels[level_index])

		output_jsons.append(curr_dict)


sys.stdout.write("\n\n\n\rOutputting segmented JSONs\n")
sys.stdout.flush()

for i, js in enumerate(output_jsons):
	sys.stdout.write("\rLevel {} out of {}".format(i+1, len(output_jsons)))
	sys.stdout.flush()
	outfile_path = os.path.join(curr_folder, 'Levels{}.json'.format(i))
	with open(outfile_path, 'w') as outfile:
		json.dump(js, outfile)

sys.stdout.write("\n\rDONE")
sys.stdout.flush()




