import sys

#sys.path.append('Scripts\\')
#sys.path.append('Lib\\pythonlib.zip')

import re,os
import com.pack

match=re.match("\d{4}-\d{2}-(\d{2})","2014-01-10")
print match.group(1)
                      
com.pack.console('12345')    