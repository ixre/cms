import clr
clr.AddReference("j6.cms");
from J6.Data import DataBaseAccess

def read(sql):
	dba = DataBaseAccess()