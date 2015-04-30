# -*- encoding: utf-8 -*-
import os
import thread
import time
import urllib2

config={'sites':(
            "www.jin-ec.com",
            "blog.ops.cc",
            "www.0345.cc",
            "www.shyishaoguanggao.com",
            "www.gdnfgy.com"),
        'duration':20
    }

print u'--- 程序预热 --'

def loadSite(siteUrl):
    try:
        urllib2.urlopen('http://%s'%(siteUrl))
    except Exception,ex:
        return 'Error,%s'%(ex.message)

    return 'OK'  

def hotSites(*site):
    print '\n%s:开始预热...'%(time.asctime(time.localtime(time.time())))
    siteLeng=len(site)
    for i in xrange(0,siteLeng):
        print '%d:%s'%(i+1,site[i]),    #,表示连行
        print '[%s]'%(loadSite(site[i]))

    time.sleep(config['duration'])
    hotSites(*site)


thread.start_new(hotSites,(config['sites']))
