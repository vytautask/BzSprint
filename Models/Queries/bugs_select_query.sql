SELECT 
	bug_id, 
	short_desc, 
	cf_duration, 
	priority, 
	CAST(cf_scrum_importance AS SIGNED INT), 
	COALESCE(cf_octopusfeature, cf_foxusfeature) as feature, products.name 
FROM bugs inner join products on products.id = bugs.product_id 
WHERE product_id IN (31, 28) AND bug_status IN ('ASSIGNED', 'REOPENED', 'IN PROGRESS', 'NEW') and cf_scrum_sprint = @sprintName 
ORDER BY CAST(cf_scrum_importance AS SIGNED INT) desc, priority					
