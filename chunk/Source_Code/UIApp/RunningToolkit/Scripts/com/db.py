import clr
clr.AddReference("ops.cms");
from Ops.Data import DataBaseAccess

def read(sql):
	dba = DataBaseAccess()