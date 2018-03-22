# 696_Project

Repository for progress done for the 696 mapping project.

## Getting Started

### OSMtoGraphviz

This parses the osm file and outputs a file that can be run in graphviz. This provided the most reliable way to create graphs of any significant size. Use neato, fdp, or dot with the -Kfdp flag on the resulting output dot file. Example below.

```
neato.exe -n -Tsvg .\map.dot -o .\map.svg
```

The program scaled the lat and long coordinates given a size (currently 4096x4096).

#### Output
The following osm map export is used as an example: 

![alt text](/mapexport.PNG)

This results in the following output. A screenshot of the exported portion of the map is shown.

![alt text](/both.png)
