Syncronizer 
- Class that holds all the logic form synchronization
- Needs to start with valid inputs to run the program
    - Folders must exist
    - syncInterval must be greater than 0
- Synchronization logic:
    - Add folders not present in destination folder that are in source 
    - Add files not present in destination folder that are in source
    - Remove files from source folder that are not in destination folder  
    - Remove folders from source folder that are not in destination folder
  
Programs.cs
    - Contains the UI/loop
    - Menu
        - change folders path
        - change interval
    - Feedback of what happening during sync