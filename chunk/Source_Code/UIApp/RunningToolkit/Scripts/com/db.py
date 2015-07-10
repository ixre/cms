import clr
clr.AddReference("ops.cms");
from J6.Data import DataBaseAccess

def read(sql):
	dba = DataBaseAccess()