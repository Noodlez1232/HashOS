#Notes


Put this in


```

                    //Go through all the arguments
                    foreach (var argument in tmpStringArray)
                    {
                        //If the argument starts with the variable tag then....
                        if (argument.StartsWith(variableTag))
                        {
                            //Go through all the variables
                            foreach (var variable in variables)
                            {
                                //Check if the variable is valid
                                if (variable.Substring(variableTag.Length) == variables[k].Name)
                                {

                                }
                            }
                        }
                    }
  ```
