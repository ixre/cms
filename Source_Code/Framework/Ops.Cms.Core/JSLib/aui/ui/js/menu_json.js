var menu = [{
    id: 'home', text: '网站收藏', uri: '#home', childs: [
        {
            text: '车辆管理', uri: '', toggle: true, iconPos: '-168|0',
            childs: [
                { text: '车辆录入', uri: '/admin/car/AddCarProfile' },
                { text: '车辆维护', uri: '/admin/car/carinfolist' },
                { text: '车辆在库情况统计', uri: '/admin/car/carstatuslist' },
                { text: '车辆进出记录查询', uri: '/admin/car/carloglist' },
                { text: '车辆状态查询测试页面', uri: '/admin/car/testcarstatus' }
            ]
        }, {
            text: '网站收藏', uri: '', toggle: true, iconPos: '-240|0',
            childs: [
                { text: '百度', uri: '/login' },
              { text: '雅虎中国', uri: '/login' }]
        }, {
            text: '车场管理', uri: '', toggle: true, iconPos: '-168|0',
            childs: [
                { text: '车辆在库情况统计', uri: '?module=file&action=filemanager' }]
        }, {
            text: '数据报表', uri: '', toggle: true, iconPos: '-24|-48',
            childs: [
                { text: '车辆录入', uri: '?module=link&action=list&type=1' },
                { text: '车辆维护', uri: '?module=link&action=list&type=2' },
                { text: '车辆在库情况统计', uri: '?module=link&action=list&type=3' }]
        },
        {
            text: '人员管理', uri: '', toggle: true, iconPos: '-168|0', childs:
            [
                { text: '车辆录入', uri: '?module=archive&action=tagsindex' },
                { text: '车辆进出记录查询', uri: '?module=plugin&action=ReplaceTags' }]
        }]
}, {
    id: 'archives', text: '界面设计', uri: '#archives',
    childs: [{
        text: '模板管理', uri: '', toggle: true, iconPos: '-264|0',
        childs: [
            { text: '模板管理', uri: '?module=template&action=templates' },
            { text: '模板编辑', uri: '?module=template&action=edit' },
            { text: '模板设置', uri: '?module=template&action=settings' }]
    }]
}, {
    id: 'system', text: '系统管理', uri: '#system',
    childs: [{
        text: '数据管理', uri: '', toggle: true, iconPos: '-432|0',
        childs: [{ text: '数据表', uri: '?module=table&action=all' },
            { text: '词典设置', uri: '?module=file&action=editfile&path=config/settings.conf' }]
    }, {
        text: '用户权限', uri: '', toggle: true, iconPos: '-432|0',
        childs: [
            { text: '用户管理', uri: '?module=user&action=userindex' }]
    },
    {
        text: '系统设置', uri: '', toggle: true, iconPos: '-432|0', childs: [
          { text: '系统设置', uri: '?module=system&action=appconfig&item=1' },
          { text: '站点管理', uri: '?module=site&action=index' },
          { text: '系统补丁', uri: '?module=system&action=patch' }]
    }, {
        text: '帮助指南', uri: '', toggle: false, iconPos: '-432|0', childs: [
            { text: '帮助', uri: 'http://www.ops.cc/cms/help/' },
            { text: '关于系统', uri: 'http://www.ops.cc/cms/about/' }]
    }]
}, {
    id: 'app', text: '插件', uri: '#app',
    childs: [{
        text: '插件管理', uri: '', toggle: true, iconPos: '-358|-144',
        childs: [
            { text: '管理插件', uri: '?module=plugin&action=dashboard' },
            { text: '插件商店', uri: '?module=plugin&action=shop' }]
    }, { text: 'APPS', uri: '', toggle: true, iconPos: '-24|-48', childs: [] }]
}];

/*
username = '管理员';
groupname = '超级管理员';
ip = '110.80.63.164';
address = '未知';
*/

window.md = menu;