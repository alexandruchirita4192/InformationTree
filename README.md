# InformationTree

InformationTree is a program used to organize tasks into a tree (breaking a task into subtasks).

![Information Tree program](https://i.imgur.com/GXAbH7x.png)

Adding detail window (subject to change because it is still a work in progress starting from RicherTextBox):
![RicherTextBox detail window](https://i.imgur.com/rspPVTq.png)

InformationTree has a lot of features:
 - Searching for tasks
 - Adding details by double-clicking on task text upper right in "Selected task" group box (data is RTF format - can see images or tables; data can also be compressed if compressed info size is lower than uncompressed info size); data can also be encrypted using a public key and decrypted using a private key (it lets you choose a task with public/private key in details)
 - Choose tasks font, color (foreground and background), size
 - Adding tasks completion percent (0-100 %)
 - Adding tasks category, urgency number, link text
 - Filtering tasks by urgency number (minimum urgency-maximum urgency) or by tasks number (minimum task-maximum task)
 - Seeing added task date, last task change date
 - Adding time spent on each task (with possibility of counting - "Start counting"/"Stop counting (update)")
 - Clearing selected task
 - Moving tasks (up/down or moving node completely (with it's subtasks too))
 - Calculating completion percentage from leaf nodes up to root node or setting completion percentage from root node to leafs
 - Collapse/Expand all tree
 - Moving to next unfinished task
 - A drawing mechanism using elementary figures like: circle, equilateral triangle, square, and others drawn by dividing a circle into n parts and connecting the points (equilateral triangle, square)

License information:
 - A component part of InformationTree is RicherTextBox that was taken from https://www.codeproject.com/Articles/24443/RicherTextBox with license The Code Project Open License (CPOL) 1.02.
