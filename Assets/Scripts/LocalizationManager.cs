using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public Action OnLanguageChanged;

    private Dictionary<string, string> localizedText = new Dictionary<string, string>();

    public int CurrentLanguageIndex { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        int savedLang = SaveManager.Instance.data.languageIndex;
        LoadLanguage(savedLang);
    }


    public void LoadLanguage(int languageIndex)
    {
        CurrentLanguageIndex = languageIndex;
        localizedText.Clear();

        if (languageIndex == 0) // EN
        {
            //MAIN MENU
            localizedText.Add("VIBRATION_ON", "ON");
            localizedText.Add("VIBRATION_OFF", "OFF");

            localizedText.Add("PLAY", "Play");

            localizedText.Add("OPTIONS", "Options");
            localizedText.Add("MUSIC", "Music");
            localizedText.Add("SOUND FX", "Sound FX");
            localizedText.Add("SENSITIVITY", "Sensitivity");
            localizedText.Add("VIBRATION", "Vibration");
            localizedText.Add("START TUTORIAL", "Start Tutorial");
            localizedText.Add("DELETE ALL DATA", "Delete All Data");
            localizedText.Add("LANGUAGE", "Language");
            localizedText.Add("BACK", "Back");

            localizedText.Add("DELETE_WARNING_LINE", "This action will permanently delete all your progress and achievements. This action cannot be undone.");
            localizedText.Add("YES", "Yes");
            localizedText.Add("NO", "No");

            localizedText.Add("ENDINGS", "Endings");
            localizedText.Add("HERO_OF_THE_PEOPLE", "Hero of the People");
            localizedText.Add("TRAITOR_OF_THE_HOMELAND", "Traitor of the Homeland");
            localizedText.Add("IRON_FIST", "Iron Fist");
            localizedText.Add("MILITARY_COUP", "Military Coup");
            localizedText.Add("THEOCRATIC_THRALL", "Theocratic Thrall");
            localizedText.Add("EXCOMMUNICATED", "Excommunicated");
            localizedText.Add("LORD_OF_CHAOS", "Lord of Chaos");
            localizedText.Add("ABSOLUTE_ORDER", "Absolute Order");
            localizedText.Add("DAWN_OF_THE_FUTURE", "Dawn of the Future");
            localizedText.Add("END_OF_UNCERTAINTY", "End of Uncertainty");
            localizedText.Add("SILENT_BALANCE", "Silent Balance");
            localizedText.Add("UNDISCOVERED", "Undiscovered");

            localizedText.Add("EXIT", "Exit");

            //GAME SCENE
            localizedText.Add("NEXT_DAY", "Next Day");
            localizedText.Add("TRY_AGAIN", "Try Again");
            localizedText.Add("NEW_GAME", "New Game");
            localizedText.Add("MAIN_MENU", "Main Menu");
            localizedText.Add("PAUSED", "Paused");
            localizedText.Add("NAME", "Name:");
            localizedText.Add("AGE", "Age:");
            localizedText.Add("GENDER", "Gender:");
            localizedText.Add("BIRTHPLACE", "Birthplace:");
            localizedText.Add("ILLNESS_OR_INJURY", "Is there any illness or injury?");
            localizedText.Add("BELONGINGS", "Do you have any belongings?");
            localizedText.Add("WHERE_FROM", "Where did you come from?");
            localizedText.Add("WHY_DID_YOU_COME", "Why did you come?");
            localizedText.Add("APPROVE", "Approve");
            localizedText.Add("REJECT", "Reject");
            localizedText.Add("LIGHT_CAPSUL", "Light   Capsul");

            localizedText.Add("MALE", "Male");
            localizedText.Add("FEMALE", "Female");

            localizedText.Add("INJURED", "Injured");
            localizedText.Add("NO_INJURY", "No");

            localizedText.Add("REASON_SHELTER", "I'm looking for a place to take shelter.");
            localizedText.Add("REASON_CHEF", "I'm a chef, I can cook for you.");
            localizedText.Add("REASON_HUNGRY", "I'm very hungry; I haven't eaten anything in days.");
            localizedText.Add("REASON_DANGER", "It's very dangerous outside, I need to protect myself.");
            localizedText.Add("REASON_FAMILY", "They said I could find my family here.");
            localizedText.Add("REASON_ALONE", "I have no one left, I am alone.");
            localizedText.Add("REASON_FRIENDS", "I'm looking for my old friends.");
            localizedText.Add("REASON_SOLDIER", "I'm a former soldier, I can guard the door for you.");
            localizedText.Add("REASON_ANY_JOB", "I can do any job, please let me in.");
            localizedText.Add("REASON_ENTER", "I just... want to enter.");
            localizedText.Add("REASON_PATH", "My path led me here.");
            localizedText.Add("REASON_DARKNESS", "I'm just running away from the darkness.");
            localizedText.Add("REASON_CURIOUS", "I wondered what was in there.");
            localizedText.Add("REASON_WATCH", "To watch you.");
            localizedText.Add("REASON_WHISPER", "The walls stopped whispering.");

            //BOOK
            localizedText.Add("CRITERIA_NORMAL_TITLE", "Standard Human Criteria (NORMAL)");
            localizedText.Add("CRITERIA_NORMAL_DESC",
    "Individuals who meet all of the following criteria may be APPROVED.\n\n" +
    "• No inconsistencies are found in the documents.\n" +
    "• No abnormal discoloration of the skin is observed.\n" +
    "• Body temperature is between 36.0°C and 37.5°C.\n" +
    "• Light produces a normal shadow.\n" +
    "• The image inside the capsule appears normal.");

            localizedText.Add("CRITERIA_SUSPICIOUS_TITLE", "Signs Requiring Attention (SUSPICIOUS)");
            localizedText.Add("CRITERIA_SUSPICIOUS_DESC",
    "Even one of the following signs indicates that the individual may be a “Suret”, and additional questioning is required before making a decision.\n\n" +
    "• Inconsistencies in the information provided during questioning.\n" +
    "• The individual claims to be injured, but their gait or posture contradicts this claim.\n" +
    "• Yellow discoloration of the skin.");


            localizedText.Add("CRITERIA_DANGER_TITLE", "Suret Trace (DANGER)");
            localizedText.Add("CRITERIA_DANGER_DESC",
    "Any of the following signs indicate that the individual is a Suret and must be REJECTED IMMEDIATELY.\n\n" +
    "• Possessing a non-human age.\n" +
    "• Red or white eye coloration is observed.\n" +
    "• Green discoloration of the skin.\n" +
    "• Body temperature below 36.0°C or above 37.5°C.\n" +
    "• Light produces an abnormal shadow.\n" +
    "• No image is visible inside the capsule.");

            // Thermometer Unit
            localizedText.Add("THERMO_UNIT_TITLE", "Thermometer Unit");
            localizedText.Add("THERMO_UNIT_DESC",
                "The thermometer measures the subject’s body temperature to detect irregularities.\n" +
                "Readings outside the normal human range may indicate abnormality, though the device can occasionally produce inaccurate results.");

            // Light Scanner Unit
            localizedText.Add("LIGHT_SCANNER_TITLE", "Light Scanner Unit");
            localizedText.Add("LIGHT_SCANNER_DESC",
                "The light scanner analyzes the subject’s shadow under focused illumination.\n" +
                "A stable shadow is expected in humans, while distortion or fluctuation may suggest an anomaly.\n" +
                "Minor observation errors may occur.");

            // Capsule Imaging Unit
            localizedText.Add("CAPSULE_IMAGING_TITLE", "Capsule Imaging Unit");
            localizedText.Add("CAPSULE_IMAGING_DESC",
                "The capsule imaging unit reveals the subject’s true form.\n" +
                "It is the most dependable detection tool available.\n" +
                "Failure to produce an image should be treated as a serious warning.");

            // Error Detector
            localizedText.Add("ERROR_DETECTOR_TITLE", "Error Detector");
            localizedText.Add("ERROR_DETECTOR_DESC",
                "System tolerance is limited; you are allowed only 2 mistakes.\n" +
                "On the third error, the system triggers an irreversible terminal failure, resetting all progress made during the day and restarting the protocols.\n" +
                "Choose carefully.\n" +
                "Keep your eyes on the detector.");

            // Equipment Usage
            localizedText.Add("EQUIPMENT_USAGE_TITLE", "Equipment Usage");
            localizedText.Add("EQUIPMENT_USAGE_DESC",
                "Your station is equipped with three primary diagnostic tools: the Thermometer Unit, the Light Scanner Unit, and the Capsule Imaging Unit.\n" +
                "Each tool has a limited number of uses.\n" +
                "Keep an eye on the on-screen counters; once a tool runs out of uses, it cannot be used again until the end of the day.");

            // Suret Pressure Warning
            localizedText.Add("SURET_PRESSURE_WARNING_DESC",
                "If you pressure a Suret with diagnostic tools excessively, it may attack you!");

            // Cadaver Wharf
            localizedText.Add("LOCATION_CADAVER_WHARF_DESC",
                "Once a busy trading port, it’s now a graveyard for washed-up ships and bodies. Survivors here scavenge the tides, but the stench of decay never leaves the air.");

            // Broken Radio Station
            localizedText.Add("LOCATION_BROKEN_RADIO_DESC",
                "Perched atop a rugged hill, this camp is trying to catch signals from the world. Even though they haven’t found any, they listen carefully to the silence of the apocalypse. Now they are trying to change the city with the machines in their hands.");

            // Ash Granary
            localizedText.Add("LOCATION_ASH_GRANARY_DESC",
                "Instead of grain, these massive silos are filled with the gray remains of the old world.\n" +
                "It’s a dry, choking place where survivors sift through ash for anything reusable.");

            // Rusty Cog Valley
            localizedText.Add("LOCATION_RUSTY_COG_DESC",
                "Built within the ruins of a massive industrial complex.\n" +
                "The ground is littered with iron, and the 'clank' of failing machinery is the only music they have left.");

            // Seven Graves Junction
            localizedText.Add("LOCATION_SEVEN_GRAVES_DESC",
                "A crossroad camp marked by seven nameless headstones.\n" +
                "It’s a neutral ground where travelers rest, but no one stays for an eighth night.");

            // Crow’s Perch
            localizedText.Add("LOCATION_CROWS_PERCH_DESC",
                "A lookout post built on a precariously tall crane.\n" +
                "The inhabitants see everything coming from miles away, living high above the dangers of the ground.");

            // Rusty Altar
            localizedText.Add("LOCATION_RUSTY_ALTAR_DESC",
                "A camp set up around a mysterious, decaying steel monument.\n" +
                "The campers live far from machines, with religious fervor, hoping that one day they will wake up.");

            // Lonely Lumber Camp
            localizedText.Add("LOCATION_LONELY_LUMBER_DESC",
                "Hidden deep within a blackened forest where the trees no longer grow. It’s a silent, fog shrouded place where the sound of an axe is a warning to intruders.");

            // Muddy Well Village
            localizedText.Add("LOCATION_MUDDY_WELL_DESC",
                "Built around the only water source for miles, though the water is thick and bitter. Life here is a constant struggle against those who want to seize it.");

            // Deadzone Garrison
            localizedText.Add("LOCATION_DEADZONE_GARRISON_DESC",
                "A militaristic outpost guarding the edge of an uninhabitable wasteland. They are the last line of defense against the horrors that crawl out of the void.");

            // Eclipse Hollow
            localizedText.Add("LOCATION_ECLIPSE_HOLLOW_DESC",
                "A camp situated deep within a naturally lightless cavern system.\n" +                
                "They believe that light is a lie told by a dying world.");

            // Bedlam Asylum
            localizedText.Add("LOCATION_BEDLAM_ASYLUM_DESC",
                "A chaotic settlement built on the ruins of an old psychiatric ward. There are no leaders here, only whispers.The residents live in a never ending collective nightmare.");

            //GAME EVENT MANAGER
            localizedText.Add("EVENT_5_DESC",
"The Deadzone Garrison camp wants to confiscate half of your camp’s supplies in order to increase border security.");

            localizedText.Add("EVENT_5_OPTION_A", "Give");
            localizedText.Add("EVENT_5_OPTION_B", "Not to give");

            localizedText.Add("EVENT_5_FEEDBACK_A",
            "Suret arrivals have decreased, but your camp is furious with you.");

            localizedText.Add("EVENT_5_FEEDBACK_B",
            "Surets are still around, but your camp respects you.");

            localizedText.Add("EVENT_8_DESC",
"The Broken Radio Station camp has found an ancient technology on your border. The Rusty Altar camp, however, claims that it is a 'demonic idol' and insists that it must be handed over and destroyed.");

            localizedText.Add("EVENT_8_OPTION_A", "Give to\nRusty Altar");
            localizedText.Add("EVENT_8_OPTION_B", "Give to\nBroken Radio Station");

            localizedText.Add("EVENT_8_FEEDBACK_A",
            "You are now a believer.");

            localizedText.Add("EVENT_8_FEEDBACK_B",
            "The ancient technology has weakened your diagnostic tools; their reliability is reduced.");

            localizedText.Add("EVENT_11_DESC",
"The Bedlam Asylum camp wants to play music in front of your camp. This will keep your people awake at night.");

            localizedText.Add("EVENT_11_OPTION_A", "Accept");
            localizedText.Add("EVENT_11_OPTION_B", "Reject");

            localizedText.Add("EVENT_11_FEEDBACK_A",
            "Your camp can’t sleep, but you're having fun.");

            localizedText.Add("EVENT_11_FEEDBACK_B",
            "Bedlam Asylum’s games are over. You’re now their sole amusement.");

            localizedText.Add("EVENT_14_DESC",
"The Rusty Altar demands that all soldiers in the Deadzone Garrison lay down their weapons and pray. The soldiers see this as an insult.");

            localizedText.Add("EVENT_14_OPTION_A", "Support\nRusty Altar");
            localizedText.Add("EVENT_14_OPTION_B", "Support\nDeadzone Garrison");

            localizedText.Add("EVENT_14_FEEDBACK_A",
            "Through faith, people found peace.");

            localizedText.Add("EVENT_14_FEEDBACK_B",
            "The soldiers now trust you.");

            localizedText.Add("EVENT_17_DESC",
"The Broken Radio Station wants to conduct genetic tests on volunteers from Bedlam Asylum. The lunatics see this as a great adventure.");

            localizedText.Add("EVENT_17_OPTION_A", "Authorize\ngenetic testing");
            localizedText.Add("EVENT_17_OPTION_B", "Block\ngenetic test");

            localizedText.Add("EVENT_17_FEEDBACK_A",
            "You watched the grand adventure with enjoyment, but your camp fears when it will be their turn.");

            localizedText.Add("EVENT_17_FEEDBACK_B",
            "You protected your camp’s values and denied the tests.");

            localizedText.Add("EVENT_20_DESC",
            "Rusty Altar wants to collect a 'sin tax' from the people. The public is very upset.");

            localizedText.Add("EVENT_20_OPTION_A", "Allow Tax");
            localizedText.Add("EVENT_20_OPTION_B", "Reject");

            localizedText.Add("EVENT_20_FEEDBACK_A",
            "Your camp has begun to lose trust in you.");

            localizedText.Add("EVENT_20_FEEDBACK_B",
            "You have rejected the path of faith.");

            localizedText.Add("EVENT_23_DESC",
"Deadzone Garrison wants to use your camp as a base to seize Broken Radio Station’s secret communication devices.");

            localizedText.Add("EVENT_23_OPTION_A", "Allow");
            localizedText.Add("EVENT_23_OPTION_B", "Refuse");

            localizedText.Add("EVENT_23_FEEDBACK_A",
            "Broken Radio Station was heavily damaged; your diagnostic tools are now less powerful.");

            localizedText.Add("EVENT_23_FEEDBACK_B",
            "Broken Radio Station has grown stronger; your diagnostic tools are now more reliable.");

            localizedText.Add("EVENT_26_DESC",
"Bedlam Asylum is planning a major prank on Rusty Altar’s holy day. They want you to leave the gates open.");

            localizedText.Add("EVENT_26_OPTION_A", "Open\ncamp gates");
            localizedText.Add("EVENT_26_OPTION_B", "Close\ncamp gates");

            localizedText.Add("EVENT_26_FEEDBACK_A",
            "The prank was enjoyable, yet more Surets are coming.");

            localizedText.Add("EVENT_26_FEEDBACK_B",
            "You stayed safe, but missed the entertainment.");

            localizedText.Add("EVENT_29_DESC",
"Broken Radio Station offers to upgrade your diagnostic tools. Usability will increase, but the noise will attract more suspicious individuals.");

            localizedText.Add("EVENT_29_OPTION_A", "Accept");
            localizedText.Add("EVENT_29_OPTION_B", "Reject");

            localizedText.Add("EVENT_29_FEEDBACK_A",
            "Diagnostic tools improved, yet they draw increased suspicious activity.");

            localizedText.Add("EVENT_29_FEEDBACK_B",
            "Everything seems to be going well. I hope so...");

            localizedText.Add("EVENT_32_DESC",
"Bedlam Asylum is attacking Deadzone Garrison’s ammunition depot. Whose side will you take?");

            localizedText.Add("EVENT_32_OPTION_A", "Help\nDeadzone Garrison");
            localizedText.Add("EVENT_32_OPTION_B", "Support chaos");

            localizedText.Add("EVENT_32_FEEDBACK_A",
            "Bedlam Asylum attacked you; your diagnostic tools now have limited uses.");

            localizedText.Add("EVENT_32_FEEDBACK_B",
            "Your camp will now face external threats alone.");

            //SITUATION CANVAS
            localizedText.Add("DAILY_REPORT", "Daily Report!");

            localizedText.Add("APPROVED", "APPROVED:");
            localizedText.Add("REJECT2", "REJECT:");

            localizedText.Add("YOU_FAILED", "YOU FAILED!");
            localizedText.Add("FAILED_TEXT", "Humanity’s last stronghold has fallen as the result of an unacceptable chain of negligence. The gatekeeper’s failure to distinguish Surets from humans compounded by a series of critical mistakes opened the door to a catastrophe that seeped deep into the heart of our shelters. The Suret threat is no longer confined to the ruins beyond the walls. Because of your faulty decisions, they now sit at our tables, stand in the next room, and linger just behind the reflection in the mirror. The camp has officially declared that purging the infiltrators is impossible and that biological security has been completely lost. The gate once believed to be humanity’s salvation has become the gravestone of our species. There is no longer any way to know who truly breathes and who is merely a flawless imitation. Humanity’s last light has been extinguished, drowned in the darkness of your carelessness.");
            localizedText.Add("YOU_DIED", "YOU DIED!");
            localizedText.Add("DIED_TEXT", "The screams echoing from the interrogation room once again proved how fragile humanity truly is. The gatekeeper’s excessive insistence and relentless questioning, meant to strip away the false mask of the Suret before him, ended in a fatal mistake. Scientists had warned time and time again: when cornered and pushed beyond their limits, Surets abandon their human mimicry and revert to pure predators. Unfortunately, the gatekeeper crossed that delicate line, triggering the hunting instinct of the creature across the table. Blood splattered across the walls and shredded files silently screamed the price of carelessness and greedy curiosity. Falling from hunter to prey became your final mistake in this world.");

            localizedText.Add("DAY_1_END",
"Deadzone Garrison dug new trenches along the wasteland border and trained its soldiers. Purge Camp tightened Suret detection at the city gates, allowing only civilians deemed safe to enter.");

            localizedText.Add("DAY_2_END",
            "Rusty Altar held prolonged rituals around the rusted monument. Broken Radio Station dismantled old machines, attempting to decipher faint signals buried in static.");

            localizedText.Add("DAY_3_END",
            "At Bedlam Asylum, residents kept one another awake with heated arguments and nightly screams. Despite the growing unrest, Purge Camp increased night patrols to maintain order.");

            localizedText.Add("DAY_4_END",
            "Rusty Altar delivered harsh sermons, declaring that weapons corrupt the soul. Viewing this rhetoric as a threat, Deadzone Garrison reinforced its defensive positions and remained on high alert.");

            localizedText.Add("DAY_5_END",
            "Deadzone Garrison demanded supplies from Purge Camp to strengthen border security. Within Purge Camp, debate rages over whether this request would fortify the defenses or weaken the people.");

            localizedText.Add("DAY_6_END",
            "Broken Radio Station attempted to trace the source of the static noises echoing above the city. Purge Camp recalibrated its sensors to ensure the machinery noise wouldn’t interfere with border inspections.");

            localizedText.Add("DAY_7_END",
            "At Bedlam Asylum, new symbols were drawn across the walls—meanings no one could decipher. Purge Camp guards began treating mentally unstable visitors with increased caution.");

            localizedText.Add("DAY_8_END",
            "Broken Radio Station discovered an ancient technology near the border. Rusty Altar declared it a demonic idol and demanded its destruction. Purge Camp was forced to decide the fate of the technology.");

            localizedText.Add("DAY_9_END",
            "Soldiers of Deadzone Garrison reported increased movement on the wasteland side. Purge Camp extended interrogation times at the gates and raised the alarm level to counter potential infiltrations.");

            localizedText.Add("DAY_10_END",
            "Sermons at Rusty Altar grew harsher, openly declaring machines a sin. Meanwhile, Broken Radio Station quietly stockpiled components, plotting changes that could reshape the city’s order.");

            localizedText.Add("DAY_11_END",
            "Bedlam Asylum sought to hold an entertainment event by blasting loud music in front of Purge Camp. Purge Camp debated how to respond to an action that could cause sleeplessness and create serious security vulnerabilities.");

            localizedText.Add("DAY_12_END",
            "Rusty Altar accused Deadzone Garrison of being soulless machines of war. In response, Deadzone Garrison labeled this rhetoric as dangerous propaganda and increased military discipline and patrols.");

            localizedText.Add("DAY_13_END",
            "Broken Radio Station began analyzing the irregular sound recordings coming from Bedlam Asylum. Residents of Bedlam Asylum started to view these recordings as a means of communicating with unknown entities.");

            localizedText.Add("DAY_14_END",
            "Rusty Altar demanded that Deadzone Garrison soldiers lay down their weapons and pray. Purge Camp was forced to decide whether this call would weaken the city’s defenses.");

            localizedText.Add("DAY_15_END",
            "At Bedlam Asylum, residents assigned roles among themselves and invented new games. Purge Camp guards reported these behaviors as possible signs of impending chaos.");

            localizedText.Add("DAY_16_END",
            "Deadzone Garrison increased its military readiness through ammunition inspections and live fire drills. Rusty Altar began extended purification rituals in preparation for an approaching holy day.");

            localizedText.Add("DAY_17_END",
            "Scientists at Broken Radio Station planned genetic tests on the volunteers arriving from Bedlam Asylum. Purge Camp found itself on the brink of a critical decision over whether these experiments were ethical or dangerously reckless.");

            localizedText.Add("DAY_18_END",
            "Residents of Bedlam Asylum declared themselves part of a great discovery. Broken Radio Station believes the data obtained will reshape the future of humanity.");

            localizedText.Add("DAY_19_END",
            "Deadzone Garrison reclassified the camps within the city as potential threats. Concerned that growing military pressure could disrupt civilian balance, Purge Camp chose to remain an observer.");

            localizedText.Add("DAY_20_END",
            "Rusty Altar announced the collection of a sin tax from the populace. Within Purge Camp, debates erupted over whether this religious imposition would ignite widespread social unrest.");

            localizedText.Add("DAY_21_END",
            "Broken Radio Station began installing machines near the borders of Purge Camp to collect data. Guards at Purge Camp observed whether the increasing mechanical activity was influencing Suret behavior.");

            localizedText.Add("DAY_22_END",
            "Residents of Bedlam Asylum organized mock military ceremonies, assigning themselves imaginary ranks and uniforms. Deadzone Garrison dismissed these strange imitations and remained focused on real threats along the borders.");

            localizedText.Add("DAY_23_END",
            "Deadzone Garrison attempted to seize Broken Radio Station’s covert communication devices. As military movement intensified around Purge Camp, a sense of tension spread throughout the city.");

            localizedText.Add("DAY_24_END",
            "Rusty Altar delivered harsh sermons accusing the people of Purge Camp of straying from faith. Within Purge Camp, fears grew that mounting religious pressure could fracture internal stability.");

            localizedText.Add("DAY_25_END",
            "Broken Radio Station recorded the erratic behaviors coming from Bedlam Asylum as valuable data. Bedlam Asylum’s residents, however, interpreted this attention as proof that they were chosen subjects.");

            localizedText.Add("DAY_26_END",
            "Residents of Bedlam Asylum began making strange preparations for Rusty Altar’s upcoming holy day. Rusty Altar could not determine whether these actions were meant as mockery or a direct affront to the sacred.");

            localizedText.Add("DAY_27_END",
            "Broken Radio Station conducted measurements along the borders of Purge Camp, testing its systems. Within Purge Camp, no clear consensus formed on whether the collected data would truly enhance security.");

            localizedText.Add("DAY_28_END",
            "Bedlam Asylum observed Deadzone Garrison patrols and began performing erratic imitations. Deadzone Garrison failed to discern whether this was harmless madness or a calculated provocation.");

            localizedText.Add("DAY_29_END",
            "Broken Radio Station offered to upgrade Purge Camp’s diagnostic equipment. As uncertainty lingered over who would truly benefit from the increased noise, a cautious tension settled over the city.");

            localizedText.Add("DAY_30_END",
            "Tensions between Deadzone Garrison and Rusty Altar escalated into open hostility. As military patrols drew closer to prayer grounds, sermons no longer spoke of peace, but of the coming conflict.");

            localizedText.Add("DAY_31_END",
            "Broken Radio Station broadcast emergency signals across the city. Residents of Bedlam Asylum interpreted the sounds as a calling and began dispersing into the streets.");

            localizedText.Add("DAY_32_END",
            "Groups from Bedlam Asylum launched an attack on Deadzone Garrison’s ammunition depot. As military defenses responded, gunfire echoed through the city, forcing camps to choose sides.");

            localizedText.Add("DAY_33_END",
            "While Deadzone Garrison struggled to recover its losses, border defenses weakened. Panic rose at Purge Camp gates, and the distinction between human and Suret became a matter of survival.");

            localizedText.Add("DAY_34_END",
            "Rusty Altar declared the devastation a divine punishment. As Broken Radio Station machines harvested combat data, whispers spread that the war had evolved into organized chaos.");

            localizedText.Add("DAY_35_END",
            "Frontlines solidified across the city, with camps openly declaring one another enemies. As borders, faith, reason, and technology shattered, the apocalypse was no longer a possibility it was reality.");

            localizedText.Add("ENDING_TRAITOR_TITLE", "Traitor of the Homeland");

            localizedText.Add("ENDING_TRAITOR_DESC",
            "You sold your own camp to other factions. In the dead of night, you were executed in your bed by the leader of your own camp.");

            localizedText.Add("ENDING_HERO_TITLE", "Hero of the People");

            localizedText.Add("ENDING_HERO_DESC",
            "You refused to yield to external pressures. You put your people above everything else. The camp now hails you as its new leader.");

            localizedText.Add("ENDING_EXCOMMUNICATED_TITLE", "Excommunicated");

            localizedText.Add("ENDING_EXCOMMUNICATED_DESC",
            "You scorned the sacred and refused the rites. Rusty Altar branded you “cursed” and sentenced you to be hanged.");

            localizedText.Add("ENDING_THEOCRATIC_THRALL_TITLE", "Theocratic Thrall");

            localizedText.Add("ENDING_THEOCRATIC_THRALL_DESC",
            "You are no longer a Gate Guard; you are a disciple. Reason gave way to faith, and you chose to stop questioning.");

            localizedText.Add("ENDING_MILITARY_COUP_TITLE", "Military Coup");

            localizedText.Add("ENDING_MILITARY_COUP_DESC",
            "You failed to hold the border. Deadzone Garrison rolled into your camp with tanks and the first bullet was meant for you.");

            localizedText.Add("ENDING_IRON_FIST_TITLE", "Iron Fist");

            localizedText.Add("ENDING_IRON_FIST_DESC",
            "You executed every order without fault. Through your actions, the camp fell under Deadzone Garrison’s command. No longer a Gate Guard, you now stand as a high ranking commander.");

            localizedText.Add("ENDING_ABSOLUTE_ORDER_TITLE", "Absolute Order");

            localizedText.Add("ENDING_ABSOLUTE_ORDER_DESC",
            "You tolerated no chaos, no disorder. Bedlam Asylum was silenced under your rule. The world became safe, sterile, lifeless, and gray.");

            localizedText.Add("ENDING_LORD_OF_CHAOS_TITLE", "Lord of Chaos");

            localizedText.Add("ENDING_LORD_OF_CHAOS_DESC",
            "You laughed as the world burned. You no longer have a camp, only destruction remains.");

            localizedText.Add("ENDING_END_OF_UNCERTAINTY_TITLE", "End of Uncertainty");

            localizedText.Add("ENDING_END_OF_UNCERTAINTY_DESC",
            "You laid bare the secrets of Broken Radio Station and wiped it out. Fear of the experiments is gone along with humanity’s greatest possible discovery.");

            localizedText.Add("ENDING_DAWN_OF_THE_FUTURE_TITLE", "Dawn of the Future");

            localizedText.Add("ENDING_DAWN_OF_THE_FUTURE_DESC",
            "You allied with Broken Radio Station. Their advanced technology reshaped your city. The ruins of the past and the other camps were swept away, leaving your city purified and protected from the rest of the world.");

            localizedText.Add("ENDING_SILENT_BALANCE_TITLE", "Silent Balance");

            localizedText.Add("ENDING_SILENT_BALANCE_DESC",
            "You made no enemies. You belonged to no one. The world survived, but without direction.");

            //TUTORIAL
            localizedText.Add("TUTORIAL_LOOK_AROUND",
                "Before moving, get familiar with your surroundings.\nSwipe to scan the area.");

            localizedText.Add("TUTORIAL_OPEN_BOOK",
            "Now that your vision is clear, approach the object on the table and press the interact button.\nBegin your journey by opening the guidebook first.");

            localizedText.Add("TUTORIAL_READY",
            "You are now ready for your duty.\nProtect the gate, protect the city.");

            localizedText.Add("TUTORIAL_STORY_1", "This book will be your key to survival. It contains vital information to help you distinguish whether the beings you encounter are a Suret or a human, as well as manuals for operating complex devices and intelligence on other camps in the region.");
            localizedText.Add("TUTORIAL_STORY_2", "You will need this knowledge, for the world has fallen silent and the great cities have collapsed. Today, the only life left is huddled within the scattered camps behind these walls.");
            localizedText.Add("TUTORIAL_STORY_3", "You stand with Purge Camp as the last guardian of the border. Your task is clear: screen those who come to the gate and separate Humans from Surets.");
            localizedText.Add("TUTORIAL_STORY_4", "The fate of the city will be shaped by your vigilance. By night, you will confront strangers at the gate. By day, you will face the reports of The Last Word. Remember offers from other camps may be tempting, but every choice demands a price.");
            localizedText.Add("TUTORIAL_STORY_5", "The gate is waiting.\nShall we begin your first shift?");
        }
        else // TR
        {
            //MAIN MENU
            localizedText.Add("VIBRATION_ON", "AÇIK");
            localizedText.Add("VIBRATION_OFF", "KAPALI");

            localizedText.Add("PLAY", "Oyna");

            localizedText.Add("OPTIONS", "Ayarlar");
            localizedText.Add("MUSIC", "MÜZİK");
            localizedText.Add("SOUND FX", "SES EFEKTLERİ");
            localizedText.Add("SENSITIVITY", "HASSASİYET");
            localizedText.Add("VIBRATION", "TİTREŞİM");
            localizedText.Add("START TUTORIAL", "ÖĞRETİCİYİ BAŞLAT");
            localizedText.Add("DELETE ALL DATA", "Oyunu Sıfırla");
            localizedText.Add("LANGUAGE", "DİL SEÇİMİ");
            localizedText.Add("BACK", "GERİ DÖN");

            localizedText.Add("DELETE_WARNING_LINE", "BU İŞLEM TÜM İLERLEMENİ VE BAŞARILARINI KALICI OLARAK SİLECEKTİR. BU İŞLEM GERİ ALINAMAZ.");
            localizedText.Add("YES", "Evet");
            localizedText.Add("NO", "Hayır");

            localizedText.Add("ENDINGS", "Sonlar");
            localizedText.Add("HERO_OF_THE_PEOPLE", "HALKIN KAHRAMANI");
            localizedText.Add("TRAITOR_OF_THE_HOMELAND", "VATAN HAİNİ");
            localizedText.Add("IRON_FIST", "DEMİR YUMRUK");
            localizedText.Add("MILITARY_COUP", "ASKERİ DARBE");
            localizedText.Add("THEOCRATIC_THRALL", "TEOKRATİK KÖLE");
            localizedText.Add("EXCOMMUNICATED", "AFOROZ EDİLMİŞ");
            localizedText.Add("LORD_OF_CHAOS", "KAOSUN EFENDİSİ");
            localizedText.Add("ABSOLUTE_ORDER", "MUTLAK DÜZEN");
            localizedText.Add("DAWN_OF_THE_FUTURE", "GELECEĞİN ŞAFAĞI");
            localizedText.Add("END_OF_UNCERTAINTY", "BELİRSİZLİĞİN SONU");
            localizedText.Add("SILENT_BALANCE", "SESSİZ DENGE");
            localizedText.Add("UNDISCOVERED", "KEŞFEDİLMEDİ");

            localizedText.Add("EXIT", "ÇIKIŞ");

            //GAME SCENE
            localizedText.Add("NEXT_DAY", "SONRAKİ GÜN");
            localizedText.Add("TRY_AGAIN", "Tekrar Dene");
            localizedText.Add("NEW_GAME", "YENİ OYUN");
            localizedText.Add("MAIN_MENU", "ANA MENÜ");
            localizedText.Add("PAUSED", "DURAKLATILDI");
            localizedText.Add("NAME", "İsim:");
            localizedText.Add("AGE", "Yaş:");
            localizedText.Add("GENDER", "Cinsiyet:");
            localizedText.Add("BIRTHPLACE", "Doğum Yeri:");
            localizedText.Add("ILLNESS_OR_INJURY", "Sağlık sorununuz var mı?");
            localizedText.Add("BELONGINGS", "Yanınızda eşya var mı?");
            localizedText.Add("WHERE_FROM", "Nereden geldiniz?");
            localizedText.Add("WHY_DID_YOU_COME", "Neden geldiniz?");
            localizedText.Add("APPROVE", "ONAYLA");
            localizedText.Add("REJECT", "REDDET");
            localizedText.Add("LIGHT_CAPSUL", "ISIK   KAPSUL");

            localizedText.Add("MALE", "Erkek");
            localizedText.Add("FEMALE", "Kadın");

            localizedText.Add("INJURED", "Yaralı");
            localizedText.Add("NO_INJURY", "Hayır");

            localizedText.Add("REASON_SHELTER", "Sığınacak bir yer arıyorum.");
            localizedText.Add("REASON_CHEF", "Aşçıyım, sizin için yemek yapabilirim.");
            localizedText.Add("REASON_HUNGRY", "Çok açım; günlerdir hiçbir şey yemedim.");
            localizedText.Add("REASON_DANGER", "Dışarısı çok tehlikeli, kendimi korumam gerek.");
            localizedText.Add("REASON_FAMILY", "Ailemi burada bulabileceğimi söylediler.");
            localizedText.Add("REASON_ALONE", "Kimsem kalmadı, yalnızım.");
            localizedText.Add("REASON_FRIENDS", "Eski arkadaşlarımı arıyorum.");
            localizedText.Add("REASON_SOLDIER", "Eski bir askerim, kapıyı koruyabilirim.");
            localizedText.Add("REASON_ANY_JOB", "Her türlü işi yaparım, lütfen beni içeri alın.");
            localizedText.Add("REASON_ENTER", "Sadece... içeri girmek istiyorum.");
            localizedText.Add("REASON_PATH", "Yolum beni buraya getirdi.");
            localizedText.Add("REASON_DARKNESS", "Sadece karanlıktan kaçıyorum.");
            localizedText.Add("REASON_CURIOUS", "İçeride ne olduğunu merak ettim.");
            localizedText.Add("REASON_WATCH", "Sizi izlemek için.");
            localizedText.Add("REASON_WHISPER", "Duvarlar fısıldamayı bıraktı.");

            //BOOK
            localizedText.Add("CRITERIA_NORMAL_TITLE", "Standart insan kriterleri (Normal)");
            localizedText.Add("CRITERIA_NORMAL_DESC",
    "Aşağıdaki kriterlerin tamamını karşılayan bireyler onaylanabilir:\n\n" +
    "• Belgelerde herhangi bir tutarsızlık bulunmamalıdır.\n" +
    "• Deride anormal bir renk değişimi gözlenmemelidir.\n" +
    "• Vücut sıcaklığı 36.0°C ile 37.5°C arasında olmalıdır.\n" +
    "• Işık normal bir gölge oluşturmalıdır.\n" +
    "• Kapsül içindeki görüntü normal görünmelidir.");


            localizedText.Add("CRITERIA_SUSPICIOUS_TITLE", "Dikkat gerektiren belirtiler (Şüpheli)");
            localizedText.Add("CRITERIA_SUSPICIOUS_DESC",
    "Aşağıdaki belirtilerden sadece biri bile bireyin bir “suret” olabileceğini gösterir ve karar vermeden önce ek sorgulama gerekir:\n\n" +
    "• Sorgulama sırasında verilen bilgilerde tutarsızlık bulunması.\n" +
    "• Bireyin yaralı olduğunu iddia etmesine rağmen yürüyüşünün veya duruşunun bu iddiayla çelişmesi.\n" +
    "• Deride sarı renk değişimi gözlenmesi.");


            localizedText.Add("CRITERIA_DANGER_TITLE", "Suret izi (Tehlike)");
            localizedText.Add("CRITERIA_DANGER_DESC",
    "Aşağıdaki belirtilerden herhangi biri bireyin suret olduğunu gösterir ve derhal reddedilmelidir:\n\n" +
    "• İnsan dışı bir yaşa sahip olmak.\n" +
    "• Kırmızı veya beyaz göz rengi gözlenmesi.\n" +
    "• Deride yeşil renk değişimi gözlenmesi.\n" +
    "• Vücut sıcaklığının 36.0°C altında veya 37.5°C üzerinde olması.\n" +
    "• Işığın anormal bir gölge oluşturması.\n" +
    "• Kapsül içinde görüntünün bulunmaması.");

            // Termometre Ünitesi
            localizedText.Add("THERMO_UNIT_TITLE", "Termometre Ünitesi");
            localizedText.Add("THERMO_UNIT_DESC",
                "Termometre, bireyin vücut sıcaklığını ölçerek anormallikleri tespit eder.\n" +
                "Normal insan aralığının dışındaki ölçümler, anormallik gösterebilir, ancak cihaz bazen hatalı sonuçlar üretebilir.");

            // Işık Tarayıcı Ünitesi
            localizedText.Add("LIGHT_SCANNER_TITLE", "Işık Tarayıcı Ünitesi");
            localizedText.Add("LIGHT_SCANNER_DESC",
                "Işık tarayıcı, bireyin gölgesini odaklı aydınlatma altında analiz eder.\n" +
                "İnsanlarda stabil bir gölge beklenir, bozulma veya dalgalanma anomaliyi gösterebilir.\n" +
                "Küçük gözlem hataları olabilir.");

            // Kapsül Görüntüleme Ünitesi
            localizedText.Add("CAPSULE_IMAGING_TITLE", "Kapsül Görüntüleme Ünitesi");
            localizedText.Add("CAPSULE_IMAGING_DESC",
                "Kapsül görüntüleme ünitesi, bireyin gerçek formunu ortaya çıkarır.\n" +
                "Mevcut en güvenilir tespit aracıdır.\n" +
                "Görüntü elde edilemezse, ciddi bir uyarı olarak değerlendirilmelidir.");

            // Hata Dedektörü
            localizedText.Add("ERROR_DETECTOR_TITLE", "Hata Dedektörü");
            localizedText.Add("ERROR_DETECTOR_DESC",
                "Sistem toleransı sınırlıdır; yalnızca 2 hata yapabilirsiniz.\n" +
                "Üçüncü hatada sistem geri dönüşsüz bir terminal hatası tetikler, gün boyunca yapılan tüm ilerlemeleri sıfırlar ve protokolleri yeniden başlatır.\n" +
                "Dikkatli seçim yapın.\n" +
                "Dedektöre gözlerinizi sabitleyin.");

            // Ekipman Kullanımı
            localizedText.Add("EQUIPMENT_USAGE_TITLE", "Ekipman Kullanımı");
            localizedText.Add("EQUIPMENT_USAGE_DESC",
                "İstasyonunuz üç ana tanı aracına sahiptir: Termometre Ünitesi, Işık Tarayıcı Ünitesi ve Kapsül Görüntüleme Ünitesi.\n" +
                "Her aracın sınırlı sayıda kullanımı vardır.\n" +
                "Ekrandaki sayacı takip edin; bir aracın kullanımı bittiğinde, günün sonunda tekrar kullanılabilir hale gelir.");

            // Aşırı Baskı Uyarısı
            localizedText.Add("SURET_PRESSURE_WARNING_DESC",
                "Bir Suret’e tanı araçlarıyla aşırı baskı uygularsanız, saldırabilir!");

            // Cadaver Wharf
            localizedText.Add("LOCATION_CADAVER_WHARF_DESC",
                "Karaya vurmuş gemiler ve cesetlerle dolu bir mezarlıkta hayatta kalanlar, dalgalar arasında eşya arar ve burada çürüme kokusu havadan hiç gitmez.");

            // Broken Radio Station
            localizedText.Add("LOCATION_BROKEN_RADIO_DESC",
                "Sarp bir tepenin üzerinde kurulu bu kamp, dünyadaki sinyalleri yakalamaya çalışıyor. Hiçbir sinyal bulamasalar da, kıyametin sessizliğini dikkatle dinliyorlar. Şimdi ellerindeki makinelerle şehri değiştirmeye çalışıyorlar.");

            // Ash Granary
            localizedText.Add("LOCATION_ASH_GRANARY_DESC",
                "Bu dev silo, tahıl yerine eski dünyanın gri kalıntılarıyla dolu. Kurak ve boğucu bir yer, hayatta kalanlar yeniden kullanılabilecek bir şeyler bulmak için küllerin içini kazıyor.");

            // Rusty Cog Valley
            localizedText.Add("LOCATION_RUSTY_COG_DESC",
                "Büyük bir endüstriyel kompleksin kalıntıları arasında inşa edilmiştir. Zemin demirle dolu ve bozulan makinelerin 'tak tak' sesi onların kalan tek müziğidir.");

            // Seven Graves Junction
            localizedText.Add("LOCATION_SEVEN_GRAVES_DESC",
                "Yedi isimsiz mezar taşıyla işaretlenmiş bir kavşak kampıdır. Burası gezginlerin dinlendiği nötr bir alan, fakat hiç kimse sekizinci geceyi burada geçirmez.");

            // Crow’s Perch
            localizedText.Add("LOCATION_CROWS_PERCH_DESC",
                "Tehlikeli bir şekilde uzun bir vinç üzerine kurulmuş bir gözetleme noktasıdır. Sakinler her şeyi kilometrelerce uzaktan görür, yerin tehlikelerinin çok üstünde yaşarlar.");

            // Rusty Altar
            localizedText.Add("LOCATION_RUSTY_ALTAR_DESC",
                "Gizemli ve çürümekte olan bir çelik anıt etrafına kurulmuş bir kamp.\n" +
                "Kamp sakinleri makinelerden uzak, dini bir tutkuyla yaşar ve bir gün uyanacaklarını umarlar.");

            // Lonely Lumber Camp
            localizedText.Add("LOCATION_LONELY_LUMBER_DESC",
                "Artık ağaçların yetişmediği, kararmış bir ormanın derinliklerine gizlenmiş. Sessiz ve sisle kaplı bir yerdir; baltanın sesi yabancılar için bir uyarıdır.");

            // Muddy Well Village
            localizedText.Add("LOCATION_MUDDY_WELL_DESC",
                "Kilometrelerce ötede tek su kaynağı etrafına kurulmuş, fakat su yoğun ve acı. Buradaki yaşam suyu ele geçirmek isteyenlerle sürekli bir mücadele içinde");

            // Deadzone Garrison
            localizedText.Add("LOCATION_DEADZONE_GARRISON_DESC",
                "Yaşanmaz bir çorak arazinin kenarını koruyan militarist bir üs. Boşluktan çıkan dehşetlere karşı son savunma hattıdırlar.");

            // Eclipse Hollow
            localizedText.Add("LOCATION_ECLIPSE_HOLLOW_DESC",
                "Doğal olarak ışık almayan bir mağara sisteminin derinliklerine kurulmuş bir kamp. Onlara göre ışık, ölen bir dünya tarafından söylenen bir yalandır");

            // Bedlam Asylum
            localizedText.Add("LOCATION_BEDLAM_ASYLUM_DESC",
                "Eski bir psikiyatri kliniğinin kalıntıları üzerine kurulmuş kaotik bir yerleşim. Burada lider yok, sadece fısıltılar vardır. Sakinler, hiç bitmeyen kolektif bir kabusta yaşarlar.");

            //GAME EVENT MANAGER
            localizedText.Add("EVENT_5_DESC",
"DEADZONE GARRISON KAMPI, SINIR GÜVENLİĞİNİ ARTIRMAK İÇİN KAMPININ ERZAKLARININ YARISINA EL KOYMAK İSTİYOR.");

            localizedText.Add("EVENT_5_OPTION_A", "VER");
            localizedText.Add("EVENT_5_OPTION_B", "VERME");

            localizedText.Add("EVENT_5_FEEDBACK_A",
            "SURET GELİŞLERİ AZALDI, FAKAT KAMPIN SANA ÖFKELİ.");

            localizedText.Add("EVENT_5_FEEDBACK_B",
            "SURETLER HÂLÂ ORTALIKTA, ANCAK KAMPIN SANA SAYGI DUYUYOR.");

            localizedText.Add("EVENT_8_DESC",
            "BROKEN RADIO STATION KAMPI SINIRINDA KADİM BİR TEKNOLOJİ BULDU. ANCAK RUSTY ALTAR KAMPI BUNUN 'ŞEYTANİ BİR PUT' OLDUĞUNU İDDİA EDİYOR VE DERHAL KENDİLERİNE TESLİM EDİLİP YOK EDİLMESİNİ İSTİYOR.");

            localizedText.Add("EVENT_8_OPTION_A", "RUSTY ALTAR'A\nVER");
            localizedText.Add("EVENT_8_OPTION_B", "BROKEN R.S'A\nVER");

            localizedText.Add("EVENT_8_FEEDBACK_A",
            "ARTIK BİR İNANAN OLDUN.");

            localizedText.Add("EVENT_8_FEEDBACK_B",
            "KADİM TEKNOLOJİ TANI ARAÇLARINI ZAYIFLATTI; GÜVENİLİRLİKLERİ AZALDI.");

            localizedText.Add("EVENT_11_DESC",
            "BEDLAM ASYLUM KAMPI, KAMPININ ÖNÜNDE MÜZİK ÇALMAK İSTİYOR. BU DURUM İNSANLARINI GECE BOYUNCA UYKUSUZ BIRAKACAK.");

            localizedText.Add("EVENT_11_OPTION_A", "KABUL ET");
            localizedText.Add("EVENT_11_OPTION_B", "REDDET");

            localizedText.Add("EVENT_11_FEEDBACK_A",
            "KAMPIN UYUYAMIYOR AMA EĞLENİYORSUN.");

            localizedText.Add("EVENT_11_FEEDBACK_B",
            "BEDLAM ASYLUM’UN OYUNLARI BİTTİ. ARTIK ONLARIN TEK EĞLENCESİ SENSİN.");

            localizedText.Add("EVENT_14_DESC",
            "RUSTY ALTAR KAMPI, DEADZONE GARRISON ASKERLERİNİN SİLAHLARINI BIRAKIP DUA ETMELERİNİ İSTİYOR. ASKERLER BUNU HAKARET OLARAK GÖRÜYOR.");

            localizedText.Add("EVENT_14_OPTION_A", "RUSTY ALTAR'I\nDESTEKLE");
            localizedText.Add("EVENT_14_OPTION_B", "DEADZONE GARRISON'I\nDESTEKLE");

            localizedText.Add("EVENT_14_FEEDBACK_A",
            "İNANÇ SAYESİNDE İNSANLAR HUZUR BULDU.");

            localizedText.Add("EVENT_14_FEEDBACK_B",
            "ASKERLER ARTIK SANA GÜVENİYOR.");

            localizedText.Add("EVENT_17_DESC",
            "BROKEN RADIO STATION KAMPI, BEDLAM ASYLUM'DAN GELEN 'GÖNÜLLÜLER' ÜZERİNDE GENETİK TESTLER YAPMAK İSTİYOR. DELİLER BUNU BÜYÜK BİR MACERA OLARAK GÖRÜYOR.");

            localizedText.Add("EVENT_17_OPTION_A", "GENETİK TESTİ\nONAYLA");
            localizedText.Add("EVENT_17_OPTION_B", "GENETİK TESTİ\nENGELLE");

            localizedText.Add("EVENT_17_FEEDBACK_A",
            "BÜYÜK MACERAYI KEYİFLE İZLEDİN, ANCAK KAMPIN SIRANIN NE ZAMAN KENDİLERİNE GELECEĞİNDEN KORKUYOR.");

            localizedText.Add("EVENT_17_FEEDBACK_B",
            "KAMPININ DEĞERLERİNİ KORUDUN VE TESTLERİ REDDETTİN.");

            localizedText.Add("EVENT_20_DESC",
            "RUSTY ALTAR KAMPI HALKTAN 'GÜNAH VERGİSİ' TOPLAMAK İSTİYOR. HALK BU DURUMA OLDUKÇA TEPKİLİ.");

            localizedText.Add("EVENT_20_OPTION_A", "VERGİYE İZİN VER");
            localizedText.Add("EVENT_20_OPTION_B", "REDDET");

            localizedText.Add("EVENT_20_FEEDBACK_A",
            "KAMPIN SANA OLAN GÜVENİNİ KAYBETMEYE BAŞLADI.");

            localizedText.Add("EVENT_20_FEEDBACK_B",
            "İNANÇ YOLUNU REDDETTİN.");

            localizedText.Add("EVENT_23_DESC",
            "DEADZONE GARRISON KAMPI, BROKEN RADIO STATION’IN GİZLİ İLETİŞİM CİHAZLARINA EL KOYMAK İÇİN KAMPINI ÜS OLARAK KULLANMAK İSTİYOR.");

            localizedText.Add("EVENT_23_OPTION_A", "İZİN VER");
            localizedText.Add("EVENT_23_OPTION_B", "REDDET");

            localizedText.Add("EVENT_23_FEEDBACK_A",
            "BROKEN RADIO STATION AĞIR HASAR ALDI; TANI ARAÇLARIN ARTIK DAHA ZAYIF.");

            localizedText.Add("EVENT_23_FEEDBACK_B",
            "BROKEN RADIO STATION GÜÇLENDİ; TANI ARAÇLARIN ARTIK DAHA GÜVENİLİR.");

            localizedText.Add("EVENT_26_DESC",
            "BEDLAM ASYLUM KAMPI, RUSTY ALTAR’IN KUTSAL GÜNÜNDE BÜYÜK BİR ŞAKA PLANLIYOR. KAPILARI AÇIK BIRAKMANI İSTİYORLAR.");

            localizedText.Add("EVENT_26_OPTION_A", "KAMP KAPILARINI\nAÇ");
            localizedText.Add("EVENT_26_OPTION_B", "KAMP KAPILARINI\nKAPAT");

            localizedText.Add("EVENT_26_FEEDBACK_A",
            "ŞAKA EĞLENCELİYDİ, FAKAT DAHA FAZLA SURET GELMEYE BAŞLADI.");

            localizedText.Add("EVENT_26_FEEDBACK_B",
            "EĞLENCEYİ KAÇIRDIN AMA GÜVENDE KALDIN.");

            localizedText.Add("EVENT_29_DESC",
            "BROKEN RADIO STATION TANI ARAÇLARINI GELİŞTİRMEYİ TEKLİF EDİYOR. KULLANILABİLİRLİK ARTACAK, FAKAT SİSTEMİN ÇIKARDIĞI GÜRÜLTÜ DAHA FAZLA ŞÜPHELİ KİŞİYİ ÇEKECEK.");

            localizedText.Add("EVENT_29_OPTION_A", "KABUL ET");
            localizedText.Add("EVENT_29_OPTION_B", "REDDET");

            localizedText.Add("EVENT_29_FEEDBACK_A",
            "TANI ARAÇLARI GELİŞTİRİLDİ, FAKAT DAHA FAZLA ŞÜPHELİ AKTİVİTE ÇEKİYOR.");

            localizedText.Add("EVENT_29_FEEDBACK_B",
            "HER ŞEY YOLUNDA GÖRÜNÜYOR. UMARIM ÖYLEDİR...");

            localizedText.Add("EVENT_32_DESC",
            "BEDLAM ASYLUM KAMPI DEADZONE GARRISON’IN MÜHİMMAT DEPOSUNA SALDIRIYOR. KİMİN TARAFINI TUTACAKSIN?");

            localizedText.Add("EVENT_32_OPTION_A", "DEADZONE GARRISON'A\nYARDIM ET");
            localizedText.Add("EVENT_32_OPTION_B", "KAOSU DESTEKLE");

            localizedText.Add("EVENT_32_FEEDBACK_A",
            "BEDLAM ASYLUM KAMPI SANA SALDIRDI; TANI ARAÇLARININ KULLANIMI ARTIK SINIRLI.");

            localizedText.Add("EVENT_32_FEEDBACK_B",
            "SINIRLARI KORUYANLAR ARTIK YOK. KAMPIN DIŞ TEHDİTLERLE TEK BAŞINA YÜZLEŞECEK.");

            //SITUATION CANVAS
            localizedText.Add("DAILY_REPORT", "GÜNLÜK RAPOR!");

            localizedText.Add("APPROVED", "ONAYLANDI:");
            localizedText.Add("REJECT2", "REDDEDİLDİ:");

            localizedText.Add("YOU_FAILED", "KAYBETTİN!");
            localizedText.Add("FAILED_TEXT", "İnsanlığın son kalesi, kabul edilemez bir ihmaller zinciri sonucunda düştü. Kapı muhafızının Suretleri insanlardan ayırt edememesi ve buna eklenen kritik hatalar, felaketin sığınaklarımızın kalbine kadar sızmasına neden oldu. Suret tehdidi artık duvarların ötesindeki yıkıntılarla sınırlı değil. Hatalı kararların yüzünden artık masalarımızda oturuyor, yan odada duruyor ve aynadaki yansımamızın hemen arkasında bekliyorlar. Kamp, sızan varlıkların temizlenmesinin imkânsız olduğunu ve biyolojik güvenliğin tamamen kaybedildiğini resmen duyurdu. Bir zamanlar insanlığın kurtuluşu olduğuna inanılan kapı, şimdi türümüzün mezar taşına dönüştü. Artık kimin gerçekten nefes aldığını, kimin ise kusursuz bir taklit olduğunu bilmenin hiçbir yolu yok. İnsanlığın son ışığı, senin dikkatsizliğinin karanlığında boğularak söndü.");
            localizedText.Add("YOU_DIED", "ÖLDÜN!");
            localizedText.Add("DIED_TEXT", "Sorgu odasından yükselen çığlıklar, insanlığın ne kadar kırılgan olduğunu bir kez daha kanıtladı. Karşısındaki Suretin sahte maskesini düşürmek için gösterilen aşırı ısrar ve bitmek bilmeyen sorgu, ölümcül bir hatayla sonuçlandı. Bilim insanları defalarca uyarmıştı: Köşeye sıkıştıklarında ve sınırları zorlandığında Suretler insani taklitlerini terk eder ve saf birer avcıya dönüşür. Ne yazık ki kapı muhafızı o ince çizgiyi aştı ve masanın karşısındaki yaratığın avlanma içgüdüsünü tetikledi. Duvarlara sıçrayan kan ve paramparça olmuş dosyalar, dikkatsizliğin ve açgözlü merakın bedelini sessizce haykırdı. Avcıyken ava dönüşmek, bu dünyadaki son hatan oldu.");

localizedText.Add("DAY_1_END",
"Deadzone Garrison çorak sınır boyunca yeni siperler kazdı ve askerlerini eğitti. Purge Camp şehir kapılarında Suret tespitini sıkılaştırdı ve yalnızca güvenli görülen sivillerin girişine izin verdi.");

localizedText.Add("DAY_2_END",
"Rusty Altar paslı anıtın etrafında uzun süren ritüeller gerçekleştirdi. Broken Radio Station eski makineleri sökerek parazitin arasına gömülü zayıf sinyalleri çözmeye çalıştı.");

localizedText.Add("DAY_3_END",
"Bedlam Asylum’da sakinler geceleri çığlıklar ve tartışmalarla birbirlerini uykusuz bıraktı. Artan huzursuzluğa rağmen Purge Camp düzeni korumak için gece devriyelerini artırdı.");

localizedText.Add("DAY_4_END",
"Rusty Altar sert vaazlar vererek silahların ruhu kirlettiğini ilan etti. Bu söylemi tehdit olarak gören Deadzone Garrison savunma pozisyonlarını güçlendirdi ve yüksek alarma geçti.");

localizedText.Add("DAY_5_END",
"Deadzone Garrison sınır güvenliğini güçlendirmek için Purge Camp’ten erzak talep etti. Purge Camp içinde bu talebin savunmayı mı güçlendireceği yoksa halkı mı zayıflatacağı tartışıldı.");

localizedText.Add("DAY_6_END",
"Broken Radio Station şehir üzerinde yankılanan parazit seslerinin kaynağını bulmaya çalıştı. Purge Camp makinelerin çıkardığı seslerin sınır kontrollerini etkilememesi için sensörleri yeniden ayarladı.");

localizedText.Add("DAY_7_END",
"Bedlam Asylum’da duvarlara anlamı çözülemeyen yeni semboller çizildi. Purge Camp muhafızları zihinsel olarak dengesiz ziyaretçilere karşı daha temkinli davranmaya başladı.");

localizedText.Add("DAY_8_END",
"Broken Radio Station sınır yakınında eski bir teknoloji keşfetti. Rusty Altar bunu şeytani bir put ilan ederek yok edilmesini talep etti. Purge Camp bu teknolojinin kaderine karar vermek zorunda kaldı.");

localizedText.Add("DAY_9_END",
"Deadzone Garrison askerleri çorak tarafta artan hareketlilik bildirdi. Purge Camp olası sızmalara karşı sorgu sürelerini uzattı ve alarm seviyesini yükseltti.");

localizedText.Add("DAY_10_END",
"Rusty Altar makineleri açıkça günah ilan eden daha sert vaazlar vermeye başladı. Broken Radio Station ise şehrin düzenini değiştirebilecek planlar yaparak parçalar stokladı.");

localizedText.Add("DAY_11_END",
"Bedlam Asylum Purge Camp önünde yüksek sesli bir eğlence etkinliği düzenlemek istedi. Purge Camp bu hareketin güvenlik açıkları oluşturup oluşturmayacağını tartıştı.");

localizedText.Add("DAY_12_END",
"Rusty Altar Deadzone Garrison’ı ruhsuz savaş makineleri olmakla suçladı. Deadzone Garrison bu söylemi tehlikeli propaganda olarak nitelendirip disiplin ve devriyeleri artırdı.");

localizedText.Add("DAY_13_END",
"Broken Radio Station Bedlam Asylum’dan gelen düzensiz ses kayıtlarını analiz etmeye başladı. Bedlam Asylum sakinleri bu kayıtları bilinmeyen varlıklarla iletişim aracı olarak görmeye başladı.");

localizedText.Add("DAY_14_END",
"Rusty Altar Deadzone Garrison askerlerinden silahlarını bırakıp dua etmelerini istedi. Purge Camp bu çağrının savunmayı zayıflatıp zayıflatmayacağına karar vermek zorunda kaldı.");

localizedText.Add("DAY_15_END",
"Bedlam Asylum’da sakinler kendi aralarında roller belirleyip yeni oyunlar icat etti. Purge Camp muhafızları bu davranışları yaklaşan kaosun işareti olarak raporladı.");

localizedText.Add("DAY_16_END",
"Deadzone Garrison mühimmat kontrolleri ve tatbikatlarla askeri hazırlığını artırdı. Rusty Altar yaklaşan kutsal gün için arınma ritüellerini uzattı.");

localizedText.Add("DAY_17_END",
"Broken Radio Station bilim insanları Bedlam Asylum’dan gelen gönüllüler üzerinde genetik testler planladı. Purge Camp bu deneylerin etik mi yoksa tehlikeli mi olduğuna karar vermek zorunda kaldı.");

localizedText.Add("DAY_18_END",
"Bedlam Asylum sakinleri kendilerini büyük bir keşfin parçası ilan etti. Broken Radio Station elde edilen verilerin insanlığın geleceğini değiştireceğine inanıyor.");

localizedText.Add("DAY_19_END",
"Deadzone Garrison şehirdeki kampları potansiyel tehdit olarak yeniden sınıflandırdı. Purge Camp artan askeri baskının dengeyi bozabileceğini düşünerek gözlemci kalmayı seçti.");

localizedText.Add("DAY_20_END",
"Rusty Altar halktan günah vergisi toplanacağını duyurdu. Purge Camp bu dini dayatmanın toplumsal huzursuzluk çıkarıp çıkarmayacağını tartıştı.");

localizedText.Add("DAY_21_END",
"Broken Radio Station veri toplamak için Purge Camp sınırlarına makineler yerleştirdi. Purge Camp muhafızları artan mekanik faaliyetin Suret davranışlarını etkileyip etkilemediğini gözlemledi.");

localizedText.Add("DAY_22_END",
"Bedlam Asylum sakinleri sahte askeri törenler düzenleyerek kendilerine hayali rütbeler verdi. Deadzone Garrison bu taklitleri ciddiye almadı ve gerçek tehditlere odaklandı.");

localizedText.Add("DAY_23_END",
"Deadzone Garrison Broken Radio Station’ın gizli iletişim cihazlarına el koymaya çalıştı. Şehir genelinde askeri hareketlilik arttıkça gerilim yükseldi.");

localizedText.Add("DAY_24_END",
"Rusty Altar Purge Camp halkını inançtan sapmakla suçlayan sert vaazlar verdi. Purge Camp içinde artan dini baskının istikrarı bozabileceği konuşuldu.");

localizedText.Add("DAY_25_END",
"Broken Radio Station Bedlam Asylum’daki düzensiz davranışları değerli veri olarak kaydetti. Bedlam Asylum sakinleri ise bunu seçilmiş olduklarının kanıtı saydı.");

localizedText.Add("DAY_26_END",
"Bedlam Asylum sakinleri Rusty Altar’ın kutsal günü için tuhaf hazırlıklar yapmaya başladı. Rusty Altar bunun alay mı yoksa hakaret mi olduğunu anlayamadı.");

localizedText.Add("DAY_27_END",
"Broken Radio Station Purge Camp sınırlarında sistem testleri yaptı. Toplanan verilerin güvenliği gerçekten artırıp artırmayacağı belirsiz kaldı.");

localizedText.Add("DAY_28_END",
"Bedlam Asylum sakinleri Deadzone Garrison devriyelerini gözlemleyip düzensiz taklitler yaptı. Deadzone Garrison bunun masum delilik mi yoksa bilinçli bir provokasyon mu olduğunu anlayamadı.");

localizedText.Add("DAY_29_END",
"Broken Radio Station Purge Camp’in teşhis ekipmanlarını yükseltmeyi teklif etti. Artan makine gürültüsünün kime yarayacağı belirsizliğini korudu.");

localizedText.Add("DAY_30_END",
"Deadzone Garrison ile Rusty Altar arasındaki gerilim açık düşmanlığa dönüştü. Askeri devriyeler dua alanlarına yaklaşırken vaazlar barıştan değil çatışmadan söz etmeye başladı.");

localizedText.Add("DAY_31_END",
"Broken Radio Station şehir genelinde acil yayın sinyalleri gönderdi. Bedlam Asylum sakinleri bu sesleri bir çağrı olarak algılayıp sokaklara dağıldı.");

localizedText.Add("DAY_32_END",
"Bedlam Asylum grupları Deadzone Garrison mühimmat deposuna saldırı düzenledi. Silah sesleri şehirde yankılanırken kamplar taraf seçmek zorunda kaldı.");

localizedText.Add("DAY_33_END",
"Deadzone Garrison kayıplarını toparlamaya çalışırken sınır savunmaları zayıfladı. Purge Camp kapılarında panik yükseldi ve insan ile Suret ayrımı hayati hale geldi.");

localizedText.Add("DAY_34_END",
"Rusty Altar yıkımı ilahi bir ceza olarak ilan etti. Broken Radio Station savaş verilerini toplarken kaosun organize bir düzene dönüştüğü fısıldandı.");

localizedText.Add("DAY_35_END",
"Şehir genelinde cepheler netleşti ve kamplar açıkça birbirine düşman ilan edildi. Sınırlar, inanç, akıl ve teknoloji çökerken kıyamet artık bir ihtimal değil, gerçeğin ta kendisiydi.");

            localizedText.Add("ENDING_TRAITOR_TITLE", "VATAN HAİNİ");

            localizedText.Add("ENDING_TRAITOR_DESC",
            "Kendi kampını diğer fraksiyonlara sattın. Gecenin bir yarısı, kendi kampının lideri tarafından yatağında infaz edildin.");

            localizedText.Add("ENDING_HERO_TITLE", "HALKIN KAHRAMANI");

            localizedText.Add("ENDING_HERO_DESC",
            "Dış baskılara boyun eğmedin. Halkını her şeyin üstünde tuttun. Kamp artık seni yeni lideri olarak selamlıyor.");

            localizedText.Add("ENDING_EXCOMMUNICATED_TITLE", "AFOROZ EDİLDİN");

            localizedText.Add("ENDING_EXCOMMUNICATED_DESC",
            "Kutsalı küçümsedin ve ritüelleri reddettin. Rusty Altar seni “lanetli” ilan etti ve idama mahkûm etti.");

            localizedText.Add("ENDING_THEOCRATIC_THRALL_TITLE", "TEOKRATİK KÖLE");

            localizedText.Add("ENDING_THEOCRATIC_THRALL_DESC",
            "Artık bir Kapı Muhafızı değilsin; bir müridsin. Akıl yerini inanca bıraktı ve sorgulamamayı seçtin.");

            localizedText.Add("ENDING_MILITARY_COUP_TITLE", "ASKERİ DARBE");

            localizedText.Add("ENDING_MILITARY_COUP_DESC",
            "Sınırı tutmayı başaramadın. Deadzone Garrison tanklarla kampına girdi ve ilk kurşun sana sıkıldı.");

            localizedText.Add("ENDING_IRON_FIST_TITLE", "DEMİR YUMRUK");

            localizedText.Add("ENDING_IRON_FIST_DESC",
            "Her emri kusursuzca yerine getirdin. Eylemlerin sonucunda kamp Deadzone Garrison’ın komutası altına girdi. Artık bir Kapı Muhafızı değilsin; yüksek rütbeli bir komutansın.");

            localizedText.Add("ENDING_ABSOLUTE_ORDER_TITLE", "MUTLAK DÜZEN");

            localizedText.Add("ENDING_ABSOLUTE_ORDER_DESC",
            "Kaosa ve düzensizliğe asla tahammül etmedin. Bedlam Asylum senin yönetimin altında susturuldu. Dünya güvenli, steril, ruhsuz ve gri bir hâle geldi.");

            localizedText.Add("ENDING_LORD_OF_CHAOS_TITLE", "KAOSUN EFENDİSİ");

            localizedText.Add("ENDING_LORD_OF_CHAOS_DESC",
            "Dünya yanarken güldün. Artık bir kampın yok, geriye yalnızca yıkım kaldı.");

            localizedText.Add("ENDING_END_OF_UNCERTAINTY_TITLE", "BELİRSİZLİĞİN SONU");

            localizedText.Add("ENDING_END_OF_UNCERTAINTY_DESC",
            "Broken Radio Station’ın sırlarını açığa çıkardın ve onu yok ettin. Deneylere dair korku, insanlığın en büyük keşif ihtimaliyle birlikte ortadan kayboldu.");

            localizedText.Add("ENDING_DAWN_OF_THE_FUTURE_TITLE", "GELECEĞİN ŞAFAĞI");

            localizedText.Add("ENDING_DAWN_OF_THE_FUTURE_DESC",
            "Broken Radio Station ile ittifak kurdun. Gelişmiş teknolojileri şehrini yeniden şekillendirdi. Geçmişin kalıntıları ve diğer kamplar silindi, geriye arınmış ve dış dünyadan korunmuş bir şehir kaldı.");

            localizedText.Add("ENDING_SILENT_BALANCE_TITLE", "SESSİZ DENGE");

            localizedText.Add("ENDING_SILENT_BALANCE_DESC",
            "Hiç düşman edinmedin. Kimseye ait olmadın. Dünya hayatta kaldı, ancak bir yönü olmadı.");

            //TUTORIAL
            localizedText.Add("TUTORIAL_LOOK_AROUND",
"HAREKET ETMEDEN ÖNCE ÇEVRENE ALIŞ.\nALANI TARAMAK İÇİN KAYDIR.");

            localizedText.Add("TUTORIAL_OPEN_BOOK",
            "GÖRÜŞÜN ARTIK NET. MASADAKİ NESNEYE YAKLAŞ VE ETKİLEŞİM TUŞUNA BAS.\nYOLCULUĞUNA REHBER KİTABINI AÇARAK BAŞLA.");

            localizedText.Add("TUTORIAL_READY",
            "ARTIK GÖREVİN İÇİN HAZIRSIN.\nKAPIYI KORU, ŞEHRİ KORU.");

            localizedText.Add("TUTORIAL_STORY_1",
            "BU KİTAP HAYATTA KALMANIN ANAHTARI OLACAK. KARŞILAŞTIĞIN VARLIKLARIN BİR SURET Mİ YOKSA İNSAN MI OLDUĞUNU AYIRT ETMENE YARDIMCI OLACAK. HAYATİ BİLGİLERİN YANI SIRA, KARMAŞIK CİHAZLARI KULLANMA KILAVUZLARINI VE BÖLGEDEKİ DİĞER KAMPLARA DAİR BİLGİLER İÇERİR.");

            localizedText.Add("TUTORIAL_STORY_2",
            "BU BİLGİYE İHTİYACIN OLACAK; ÇÜNKÜ DÜNYA SESSİZLİĞE GÖMÜLDÜ VE BÜYÜK ŞEHİRLER ÇÖKTÜ. BUGÜN GERİYE KALAN TEK YAŞAM, BU DUVARLARIN ARDINDAKİ DAĞINIK KAMPLARDA TOPLANMIŞ HALDE.");

            localizedText.Add("TUTORIAL_STORY_3",
            "PURGE CAMP İLE BİRLİKTE SINIRIN SON MUHAFIZI OLARAK OLARAK DURUYORSUN. GÖREVİN NET: KAPIYA GELENLERİ KONTROL ET VE İNSANLARI SURETLERDEN AYIR.");

            localizedText.Add("TUTORIAL_STORY_4",
            "ŞEHRİN KADERİ SENİN DİKKATİNLE ŞEKİLLENECEK. GECE OLDUĞUNDA KAPIDA YABANCILARLA YÜZLEŞECEKSİN. GÜNDÜZ İSE THE LAST WORD RAPORLARIYLA KARŞILAŞACAKSIN. DİĞER KAMPLARDAN GELEN TEKLİFLER CAZİP OLABİLİR, ANCAK HER SEÇİMİN BİR BEDELİ VARDIR.");

            localizedText.Add("TUTORIAL_STORY_5",
            "KAPI SENİ BEKLİYOR.\nİLK NÖBETİNE BAŞLAMAYA HAZIR MISIN?");
        }

        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key)
    {
        if (localizedText.ContainsKey(key))
            return localizedText[key];

        return key;
    }
}
